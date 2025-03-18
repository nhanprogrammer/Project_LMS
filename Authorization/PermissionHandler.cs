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
             // var userPermissions = context.User.FindAll("Permission").Select(c => c.Value).ToList();

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

            if (userPermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
