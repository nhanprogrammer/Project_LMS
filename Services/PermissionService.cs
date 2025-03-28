using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<PermissionListGroupResponse>> GetPermissionListGroup(string key, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Mặc định 10 bản ghi mỗi trang

            // Tạo truy vấn cơ bản
            var query = _context.GroupModulePermissons
                .Where(p => p.IsDelete == false &&
                    (string.IsNullOrEmpty(key) ||
                     p.Name.Contains(key) ||
                     p.Description.Contains(key)));

            // Lấy tổng số bản ghi sau khi lọc
            int totalItems = await query.CountAsync();

            if (totalItems == 0)
            {
                throw new NotFoundException("Không tìm thấy nhóm quyền nào.");
            }

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy danh sách bản ghi theo trang
            var permissions = await query
                .OrderBy(p => p.Id) // Có thể thay đổi theo thứ tự bạn muốn
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermissionListGroupResponse
                {
                    Id = p.Id,
                    Name = p.Name ?? "NaN",
                    MemberCount = _context.Users.Count(u => u.GroupModulePermissonId == p.Id).ToString(),
                    Description = p.Description ?? "NaN"
                })
                .ToListAsync();

            // Trả về dữ liệu dạng phân trang
            return new PaginatedResponse<PermissionListGroupResponse>
            {
                Items = permissions, // Trả về danh sách chứ không phải một phần tử đơn lẻ
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<bool> SaveGroupPermission(int groupRoleId, string groupRoleName, string description, bool allPermission, List<ModulePermissionRequest> permissions)
        {
            GroupModulePermisson group;

            if (groupRoleId <= 0)
            {
                group = new GroupModulePermisson
                {
                    Name = groupRoleName,
                    Description = description
                };
                _context.GroupModulePermissons.Add(group);
                await _context.SaveChangesAsync();
                groupRoleId = group.Id;
            }
            else
            {
                group = await _context.GroupModulePermissons
                    .FirstOrDefaultAsync(g => g.Id == groupRoleId && g.IsDelete == false);

                if (group == null)
                {
                    throw new NotFoundException("Không tìm thấy nhóm quyền.");
                }

                if (group.Name != groupRoleName || group.Description != description)
                {
                    group.Name = groupRoleName;
                    group.Description = description;
                    group.UpdateAt = DateTime.Now;
                    _context.GroupModulePermissons.Update(group);
                }
            }

            var existingPermissions = await _context.ModulePermissions
                .Where(p => p.GroupRoleId == groupRoleId)
                .ToListAsync();

            var validModuleIds = new HashSet<int>(
                await _context.Modules.AsNoTracking().Select(m => m.Id).ToListAsync()
            );

            var existingPermissionsDict = existingPermissions
                .Where(p => p.ModuleId.HasValue)
                .ToLookup(p => p.ModuleId.Value);

            List<ModulePermission> newPermissions = new List<ModulePermission>();

            if (allPermission)
            {
                var moduleIdsWithPermission = existingPermissionsDict.Select(g => g.Key).ToHashSet();
                var modulesToAdd = await _context.Modules
                    .Where(m => !moduleIdsWithPermission.Contains(m.Id))
                    .ToListAsync();

                foreach (var module in modulesToAdd)
                {
                    var newPermission = new ModulePermission
                    {
                        ModuleId = module.Id,
                        GroupRoleId = groupRoleId,
                        IsView = true,
                        IsInsert = true,
                        IsUpdate = true,
                        IsDelete = true,
                        EnterScore = true
                    };
                    newPermissions.Add(newPermission);
                }

                foreach (var groupPermission in existingPermissionsDict)
                {
                    var existing = groupPermission.FirstOrDefault();
                    if (existing != null)
                    {
                        existing.IsView = true;
                        existing.IsInsert = true;
                        existing.IsUpdate = true;
                        existing.IsDelete = true;
                        existing.EnterScore = true;
                        existing.UpdateAt = DateTime.Now;
                        _context.ModulePermissions.Update(existing);
                    }
                }
            }
            else
            {
                if (permissions == null || !permissions.Any())
                {
                    throw new ArgumentNullException(nameof(permissions), "Danh sách quyền không được để trống khi allPermission là false.");
                }

                var permissionModuleIds = permissions.Select(p => p.ModuleId).ToHashSet();
                var permissionsToRemove = existingPermissions
                    .Where(p => !permissionModuleIds.Contains(p.ModuleId.Value))
                    .ToList();

                if (permissionsToRemove.Any())
                {
                    _context.ModulePermissions.RemoveRange(permissionsToRemove);
                }

                foreach (var permission in permissions)
                {
                    if (!validModuleIds.Contains(permission.ModuleId))
                    {
                        throw new BadHttpRequestException($"ModuleId {permission.ModuleId} không hợp lệ.");
                    }

                    var existingPermission = existingPermissionsDict[permission.ModuleId].FirstOrDefault();

                    if (existingPermission != null)
                    {
                        if (existingPermission.IsView != permission.IsView ||
                            existingPermission.IsInsert != permission.IsInsert ||
                            existingPermission.IsUpdate != permission.IsUpdate ||
                            existingPermission.IsDelete != permission.IsDelete ||
                            existingPermission.EnterScore != permission.EnterScore)
                        {
                            existingPermission.IsView = permission.IsView;
                            existingPermission.IsInsert = permission.IsInsert;
                            existingPermission.IsUpdate = permission.IsUpdate;
                            existingPermission.IsDelete = permission.IsDelete;
                            existingPermission.EnterScore = permission.EnterScore;
                            existingPermission.UpdateAt = DateTime.Now;
                            _context.ModulePermissions.Update(existingPermission);
                        }
                    }
                    else
                    {
                        var newPermission = new ModulePermission
                        {
                            ModuleId = permission.ModuleId,
                            GroupRoleId = groupRoleId,
                            IsView = permission.IsView,
                            IsInsert = permission.IsInsert,
                            IsUpdate = permission.IsUpdate,
                            IsDelete = permission.IsDelete,
                            EnterScore = permission.EnterScore
                        };
                        newPermissions.Add(newPermission);
                    }
                }
            }

            try
            {
                // Truy vết user để yêu cầu đăng nhập lại
                // var lsUser = await _context.Users
                //     .Where(u => u.GroupModulePermissonId == groupRoleId)
                //     .ToListAsync();

                // if (lsUser.Any())
                // {
                //     foreach (var user in lsUser)
                //     {
                //         user.PermissionChanged = true;
                //     }
                //     await _context.SaveChangesAsync();
                // }


                if (newPermissions.Any())
                {
                    await _context.ModulePermissions.AddRangeAsync(newPermissions);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Lỗi đồng thời khi cập nhật dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi lưu quyền nhóm.", ex);
            }
        }



        public async Task<GroupPermissionResponse> GetGroupPermissionById(int groupRoleId)
        {
            // Nếu groupRoleId <= 0, trả về danh sách module mặc định
            if (groupRoleId <= 0)
            {
                var modules = await _context.Modules
                    .Select(m => new { m.Id, m.DisplayName })
                    .ToListAsync();

                return new GroupPermissionResponse
                {
                    GroupRoleId = 0,
                    GroupRoleName = null,
                    Description = null,
                    Permissions = modules.Select(m => new ModulePermissionRequest
                    {
                        ModuleId = m.Id,
                        ModuleName = m.DisplayName,
                        IsView = false,
                        IsInsert = false,
                        IsUpdate = false,
                        IsDelete = false,
                        EnterScore = false
                    }).ToList()
                };
            }

            // Lấy thông tin nhóm quyền
            var groupEntity = await _context.GroupModulePermissons
                .FirstOrDefaultAsync(g => g.Id == groupRoleId && g.IsDelete == false);

            if (groupEntity == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm quyền.");
            }

            var group = new GroupPermissionResponse
            {
                GroupRoleId = groupEntity.Id,
                GroupRoleName = groupEntity.Name,
                Description = groupEntity.Description
            };

            // Lấy danh sách module (chỉ lấy cột cần thiết)
            var modulesList = await _context.Modules
                .Select(m => new { m.Id, m.DisplayName })
                .ToListAsync();

            // Lấy danh sách quyền của nhóm
            var existingPermissions = await _context.ModulePermissions
                .Where(p => p.GroupRoleId == groupRoleId && p.ModuleId.HasValue)
                .ToListAsync();

            var existingPermissionsDict = existingPermissions
                .Where(p => p.ModuleId.HasValue)
                .ToLookup(p => p.ModuleId.Value);

            // Ghép quyền vào module
            group.Permissions = modulesList.Select(m =>
            {
                var permission = existingPermissionsDict[m.Id].FirstOrDefault();
                return new ModulePermissionRequest
                {
                    ModuleId = m.Id,
                    ModuleName = m.DisplayName,
                    IsView = permission?.IsView ?? false,
                    IsInsert = permission?.IsInsert ?? false,
                    IsUpdate = permission?.IsUpdate ?? false,
                    IsDelete = permission?.IsDelete ?? false,
                    EnterScore = permission?.EnterScore ?? false
                };
            }).ToList();


            return group;
        }


        public async Task<bool> DeleteGroupPermission(int groupRoleId)
        {
            var group = await _context.GroupModulePermissons
                .FirstOrDefaultAsync(g => g.Id == groupRoleId && g.IsDelete == false);

            if (group == null)
            {
                throw new NotFoundException("Không tìm thấy nhóm quyền.");
            }

            // Đánh dấu xóa mềm
            group.IsDelete = true;
            group.UpdateAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Lỗi đồng thời khi xóa dữ liệu.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi xóa nhóm quyền.", ex);
            }
        }

        public async Task<PaginatedResponse<PermissionUserResponse>> GetPermissionUserList(string key, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Mặc định 10 bản ghi mỗi trang

            key = key?.ToLower()?.Trim(); // Chuẩn hóa key để tránh lỗi tìm kiếm

            // Tạo truy vấn cơ bản
            var query = _context.Users
                .AsNoTracking() // Tăng hiệu suất, vì không cần theo dõi Entity
                .Where(p => p.IsDelete == false &&
                    (string.IsNullOrEmpty(key) ||
                     p.FullName.ToLower().Contains(key) ||
                     p.Email.ToLower().Contains(key) ||
                     (p.GroupModulePermissonId != null &&
                      _context.GroupModulePermissons
                          .Where(g => g.Id == p.GroupModulePermissonId)
                          .Any(g => g.Name.ToLower().Contains(key)))));

            // Lấy tổng số bản ghi sau khi lọc
            int totalItems = await query.CountAsync();

            if (totalItems == 0)
            {
                throw new NotFoundException("Không tìm thấy người dùng nào.");
            }

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy danh sách bản ghi theo trang
            var permissions = await query
                .OrderBy(p => p.Id) // Có thể thay đổi theo thứ tự mong muốn
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermissionUserResponse
                {
                    Id = p.Id,
                    Name = p.FullName ?? "NaN",
                    Email = p.Email ?? "NaN",
                    GroupPermissionName = _context.GroupModulePermissons
                        .Where(g => g.Id == p.GroupModulePermissonId)
                        .Select(g => g.Name)
                        .FirstOrDefault() ?? "NaN",
                    Status = p.Disable.HasValue && p.Disable.Value ? "Đã vô hiệu hóa" : "Đang hoạt động"
                })
                .ToListAsync();

            // Trả về dữ liệu dạng phân trang
            return new PaginatedResponse<PermissionUserResponse>
            {
                Items = permissions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        public async Task<PermissionUserRequest> GetUserPermission(int userId)
        {
            // Nếu userId <= 0, trả về dữ liệu mặc định (dành cho create)
            if (userId <= 0)
            {
                return new PermissionUserRequest
                {
                    UserId = 0,
                    GroupId = 0,
                    Disable = false
                };
            }

            // Lấy thông tin user
            var user = await _context.Users
                .Where(u => u.Id == userId && u.IsDelete == false)
                .Select(u => new PermissionUserRequest
                {
                    UserId = u.Id,
                    GroupId = u.GroupModulePermissonId ?? 0, // Tránh null gây lỗi
                    Disable = u.Disable ?? false
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            return user;
        }

        public async Task<bool> SaveUserPermission(int userId, int groupId, bool disable)
        {
            // Kiểm tra groupId có tồn tại không
            bool groupExists = await _context.GroupModulePermissons.AnyAsync(g => g.Id == groupId);
            if (!groupExists)
            {
                throw new NotFoundException("Nhóm quyền không tồn tại.");
            }

            // Kiểm tra user có tồn tại không
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                // Nếu user không tồn tại, tạo mới user với quyền được cấp
                user = new User
                {
                    Id = userId, // ID có thể được gán nếu là user mới
                    GroupModulePermissonId = groupId,
                    Disable = disable
                };

                await _context.Users.AddAsync(user);
            }
            else
            {
                // Nếu user đã tồn tại, cập nhật nhóm quyền và trạng thái
                user.GroupModulePermissonId = groupId;
                user.Disable = disable;
                user.UpdateAt = DateTime.Now; // Cập nhật thời gian sửa đổi
                _context.Users.Update(user);
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
            return true; // Thành công
        }
        public async Task<bool> DeleteUser(int userId)
        {
            // Kiểm tra user có tồn tại không
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsDelete == false);

            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng.");
            }

            // Cập nhật trạng thái IsDelete thành true
            user.IsDelete = true;
            user.UpdateAt = DateTime.Now; // Cập nhật thời gian sửa đổi

            _context.Users.Update(user);

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
            return true; // Thành công
        }


        public async Task<List<string>> ListPermission(int userId)
        {
            var user = await _context.Users
                .Where(u => u.Id == userId && u.IsDelete == false && u.PermissionChanged == false && u.Disable == false)
                .Select(u => new { u.GroupModulePermissonId })
                .FirstOrDefaultAsync();

            if (user == null) return new List<string>();

            var modulePermissions = await _context.ModulePermissions
                .Where(m => m.GroupRoleId == user.GroupModulePermissonId)
                .ToListAsync();

            if (!modulePermissions.Any()) return new List<string>();

            // Load tất cả Modules trước để tránh truy vấn nhiều lần
            var moduleIds = modulePermissions.Select(m => m.ModuleId).Distinct().ToList();
            var modulesDict = await _context.Modules
                .Where(m => moduleIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Name);

            // Chuyển đổi dữ liệu
            var permissions = modulePermissions
                .Where(m => m.ModuleId.HasValue && modulesDict.ContainsKey(m.ModuleId.Value)) // ✅ Kiểm tra null
                .SelectMany(m => new[]
                {
                    m.IsView.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-VIEW" : null,
                    m.IsInsert.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-INSERT" : null,
                    m.IsUpdate.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-UPDATE" : null,
                    m.IsDelete.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-DELETE" : null,
                    m.EnterScore.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-ENTERSCORE" : null
                })
                .Where(p => p != null) // Lọc bỏ null
                .ToList();

            return permissions;
        }

    }
}