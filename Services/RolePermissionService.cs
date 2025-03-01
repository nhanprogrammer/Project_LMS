using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;

namespace Project_LMS.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public RolePermissionService(IRolePermissionRepository rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<IEnumerable<RolePermissionResponse>> GetAllAsync()
        {
            var rolePermissions = await _rolePermissionRepository.GetAllAsync();
            return rolePermissions.Select(rp => new RolePermissionResponse
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                ModuleId = rp.ModuleId,
                PermissionId = rp.PermissionId,
                CreateAt = rp.CreateAt,
                UpdateAt = rp.UpdateAt,
                UserCreate = rp.UserCreate,
                UserUpdate = rp.UserUpdate,
                IsDelete = rp.IsDelete
            });
        }

        public async Task<RolePermissionResponse> GetByIdAsync(int id)
        {
            var rolePermission = await _rolePermissionRepository.GetByIdAsync(id);
            if (rolePermission == null)
            {
                return null;
            }
            return new RolePermissionResponse
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                CreateAt = rolePermission.CreateAt,
                UpdateAt = rolePermission.UpdateAt,
                UserCreate = rolePermission.UserCreate,
                UserUpdate = rolePermission.UserUpdate,
                IsDelete = rolePermission.IsDelete
            };
        }

        public async Task<RolePermissionResponse> CreateAsync(RolePermissionRequest request)
        {
            if (request.RoleId == null || request.ModuleId == null || request.PermissionId == null)
            {
                throw new ArgumentNullException("Data cannot be null.");
            }
            var rolePermission = new RolePermission
            {
                RoleId = request.RoleId.Value,
                ModuleId = request.ModuleId.Value,
                PermissionId = request.PermissionId.Value,
                UserCreate = 1,
                IsDelete = false,
            };
            await _rolePermissionRepository.AddAsync(rolePermission);
            return new RolePermissionResponse
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                CreateAt = rolePermission.CreateAt,
                UpdateAt = rolePermission.UpdateAt,
                UserCreate = rolePermission.UserCreate,
                UserUpdate = rolePermission.UserUpdate,
                IsDelete = rolePermission.IsDelete
            };
        }

        public async Task<RolePermissionResponse> UpdateAsync(int id, RolePermissionRequest request)
        {
            var rolePermission = await _rolePermissionRepository.GetByIdAsync(id);
            if (rolePermission == null)
            {
                throw new NotFoundException("Bản ghi không tồn tại.");
            }
            if (request.RoleId == null || request.ModuleId == null || request.PermissionId == null)
            {
                throw new ArgumentNullException("Data cannot be null.");
            }
            rolePermission.RoleId = request.RoleId.Value;
            rolePermission.ModuleId = request.ModuleId.Value;
            rolePermission.PermissionId = request.PermissionId.Value;
            rolePermission.UserUpdate = 1;

            await _rolePermissionRepository.UpdateAsync(rolePermission);
            return new RolePermissionResponse
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                CreateAt = rolePermission.CreateAt,
                UpdateAt = rolePermission.UpdateAt,
                UserCreate = rolePermission.UserCreate,
                UserUpdate = rolePermission.UserUpdate,
                IsDelete = rolePermission.IsDelete
            };
        }

        public async Task<RolePermissionResponse> DeleteAsync(int id)
        {
            var rolePermission = await _rolePermissionRepository.GetByIdAsync(id);
            if (rolePermission == null)
            {
                return null;
            }
            rolePermission.IsDelete = true;
            rolePermission.UserUpdate = 1;

            await _rolePermissionRepository.UpdateAsync(rolePermission);
            return new RolePermissionResponse
            {
                Id = rolePermission.Id,
                RoleId = rolePermission.RoleId,
                ModuleId = rolePermission.ModuleId,
                PermissionId = rolePermission.PermissionId,
                CreateAt = rolePermission.CreateAt,
                UpdateAt = rolePermission.UpdateAt,
                UserCreate = rolePermission.UserCreate,
                UserUpdate = rolePermission.UserUpdate,
                IsDelete = rolePermission.IsDelete
            };
        }
    }
}