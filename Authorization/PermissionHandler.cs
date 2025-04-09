using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_LMS.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly ApplicationDbContext _context;

        public PermissionHandler(IPermissionService permissionService, ApplicationDbContext context)
        {
            _permissionService = permissionService;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var email = context.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var userId = await _context.Users
                .Where(u => u.Email == email && u.IsDelete == false)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
            {
                return;
            }

            var userPermissions = await _permissionService.ListPermission(userId);

            // Kiểm tra xem requirement.Permission có chứa dấu phẩy không
            if (requirement.Permission.Contains(","))
            {
                // Tách chuỗi thành mảng các quyền
                var requiredPermissions = requirement.Permission.Split(',');

                // Logic OR: Nếu user có ít nhất một quyền trong danh sách
                if (requiredPermissions.Any(perm => userPermissions.Contains(perm.Trim())))
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                // Logic AND: Trường hợp thông thường với một quyền duy nhất
                if (userPermissions.Contains(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}