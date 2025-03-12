using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IPermissionService
    {
        Task<PaginatedResponse<PermissionListGroupResponse>> GetPermissionListGroup(string key, int pageNumber, int pageSize);
        Task<bool> SaveGroupPermission(int groupRoleId, string groupRoleName, string description, bool allPermission, List<ModulePermissionRequest> permissions);
        Task<GroupPermissionResponse> GetGroupPermissionById(int groupRoleId);
        Task<bool> DeleteGroupPermission(int groupRoleId);
        Task<PaginatedResponse<PermissionUserResponse>> GetPermissionUserList(string key, int pageNumber, int pageSize);
        Task<PermissionUserRequest> GetUserPermission(int userId, int groupId, bool disable);
        Task<bool> SaveUserPermission(int userId, int groupId, bool disable);
        Task<bool> DeleteUser(int userId);
    }
}