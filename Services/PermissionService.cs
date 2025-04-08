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

            // Truy vấn cơ bản, Include để tránh truy vấn thừa
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.GroupModulePermisson) // Load GroupModulePermisson luôn
                .Where(p => p.IsDelete == false && p.Role.Name.ToUpper() == "ADMIN" &&
                    (string.IsNullOrEmpty(key) ||
                     p.FullName.ToLower().Contains(key) ||
                     p.Email.ToLower().Contains(key) ||
                     p.GroupModulePermisson.Name.ToLower().Contains(key))); // Truy vấn trực tiếp trên GroupModulePermisson

            // Đếm tổng số bản ghi
            int totalItems = await query.CountAsync();
            if (totalItems == 0)
            {
                throw new NotFoundException("Không tìm thấy người dùng nào.");
            }

            // Tính tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy danh sách bản ghi theo trang
            var permissions = await query
                .OrderBy(p => p.Id) // Có thể thay đổi thứ tự sắp xếp
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PermissionUserResponse
                {
                    Id = p.Id,
                    Name = p.FullName ?? "NaN",
                    Email = p.Email ?? "NaN",
                    GroupPermissionName = p.GroupModulePermisson.Name ?? "NaN", // Không cần truy vấn phụ
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
            var user = await _context.Users
                .Include(u => u.Role) // Load role của user
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsDelete == false);

            if (user == null)
            {
                throw new NotFoundException("Người dùng không tồn tại.");
            }

            if (user.Role.Name.ToUpper() == "TEACHER")
            {
                // Nếu là giáo viên kiểm tra có phân công giảng dạy không
                var teachingAssignments = await _context.TeachingAssignments
                    .Where(a => a.UserId == userId && a.IsDelete == false).ToListAsync();
                // Kiểm tra có phân công giảng dạy nào chưa kết thúc (ngày hiện tại lớn hơn ngày bắt đầu và nhỏ hơn ngày kết thúc)
               foreach (var assignment in teachingAssignments)
                {
                    if (assignment.StartDate <= DateTime.Now && assignment.EndDate >= DateTime.Now)
                    {
                        throw new BadHttpRequestException("Người dùng đang có phân công giảng dạy, không thể thay đổi quyền.");
                    }
                }
               
            }
           

            if (disable)
            {
                // Nếu chưa có quyền (GroupModulePermissonId == null) thì không cần thay đổi
                if (user.GroupModulePermissonId == null)
                {
                    return false;
                }

                // Nếu user đã có quyền nhưng bị disable, phục hồi vai trò trước đó
                if (user.ReRoleId.HasValue)
                {
                    user.RoleId = user.ReRoleId.Value; // Phục hồi vai trò trước đó
                }

                user.Disable = true;
            }
            else
            {
                    // ✅ Chỉ lưu `ReRoleId` nếu chưa có (tránh bị ghi đè sai)
                    if (!user.ReRoleId.HasValue)
                    {
                        user.ReRoleId = user.RoleId; // Lưu lại vai trò gốc (TEACHER/STUDENT)
                    }
                    user.RoleId = 5; // Cấp quyền ADMIN
           

                // Cập nhật nhóm quyền (không xóa khi disable)
                user.GroupModulePermissonId = groupId;
                user.Disable = false; // Kích hoạt lại user nếu trước đó bị vô hiệu hóa
            }

            // Cập nhật thời gian chỉnh sửa
            user.UpdateAt = DateTime.Now;

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

        public async Task<List<UnassignedUserResponse>> GetUnassignedUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsDelete == false && u.Role.Name.ToUpper() != "ADMIN" && u.Role.Name.ToUpper() != "SUPER-ADMIN")
                .Select(u => new UnassignedUserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName ?? "NaN",
                    UserCode = u.UserCode ?? "NaN",
                    Email = u.Email ?? "NaN"
                })
                .ToListAsync();

            return users;
        }

        public async Task<List<AvailablePermissionResponse>> GetAvailablePermissionsAsync()
        {
            var permissions = await _context.GroupModulePermissons
                .Select(g => new AvailablePermissionResponse
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync();

            return permissions;
        }

        public async Task<List<string>> ListPermission(int userId)
        {
            var permissions = new List<string>();

            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == userId && u.IsDelete == false)
                .Select(u => new { u.GroupModulePermissonId, u.Role, u.Disable })
                .FirstOrDefaultAsync();

            // Nếu user không tồn tại, trả về danh sách rỗng
            if (user == null) return permissions;

            // Nếu là SUPER-ADMIN, mặc định add quyền SUPER-ADMIN và return ngay
            if (user.Role?.Name?.ToUpper() == "SUPER-ADMIN")
            {
                permissions.Add("SUPER-ADMIN");
                return permissions;
            }

            // Luôn thêm role vào danh sách quyền
            if (!string.IsNullOrEmpty(user.Role?.Name))
            {
                permissions.Add(user.Role.Name.ToUpper());
            }

            if (user.Disable == true)
            {
                return permissions;
            }

            // Lấy danh sách quyền theo GroupModulePermissonId
            var modulePermissions = await _context.ModulePermissions
                .Where(m => m.GroupRoleId == user.GroupModulePermissonId)
                .ToListAsync();

            if (!modulePermissions.Any()) return permissions; // Nếu không có quyền nào, chỉ return ADMIN

            // Lấy danh sách module để tránh truy vấn nhiều lần
            var moduleIds = modulePermissions
                .Where(m => m.ModuleId.HasValue)
                .Select(m => m.ModuleId.Value)
                .Distinct()
                .ToList();

            var modulesDict = await _context.Modules
                .Where(m => moduleIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, m => m.Name);

            // Tạo danh sách quyền từ module permissions
            permissions.AddRange(modulePermissions
                .Where(m => m.ModuleId.HasValue && modulesDict.ContainsKey(m.ModuleId.Value))
                .SelectMany(m => new[]
                {
            m.IsView.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-VIEW" : null,
            m.IsInsert.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-INSERT" : null,
            m.IsUpdate.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-UPDATE" : null,
            m.IsDelete.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-DELETE" : null,
            m.EnterScore.GetValueOrDefault() ? $"{modulesDict[m.ModuleId.Value]}-ENTERSCORE" : null
                })
                .Where(p => p != null)); // Lọc bỏ null

            return permissions;
        }
    }
}