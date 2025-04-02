using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Project_LMS.Authorization
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            services.AddAuthorization(options =>
            {
                var permissions = new List<string>
                {
                    //Khai báo dữ liệu
                    "DATA-MNG-VIEW",
                    "DATA-MNG-INSERT",
                    "DATA-MNG-UPDATE",
                    "DATA-MNG-DELETE",
                    "DATA-MNG-ENTERSCORE",
                    //Hồ sơ học viên
                    "STUDENT-REC-VIEW",
                    "STUDENT-REC-INSERT",
                    "STUDENT-REC-UPDATE",
                    "STUDENT-REC-DELETE",
                    "STUDENT-REC-ENTERSCORE",
                    //Hồ sơ giảng viên
                    "TEACHER-REC-VIEW",
                    "TEACHER-REC-INSERT",
                    "TEACHER-REC-UPDATE",
                    "TEACHER-REC-DELETE",
                    "TEACHER-REC-ENTERSCORE",
                    //Thi cử
                    "EXAM-VIEW",
                    "EXAM-INSERT",
                    "EXAM-UPDATE",
                    "EXAM-DELETE",
                    "EXAM-ENTERSCORE",
                    //Cài dặt hệ thống
                    "SYS-SET-VIEW",
                    "SYS-SET-INSERT",
                    "SYS-SET-UPDATE",
                    "SYS-SET-DELETE",
                    "SYS-SET-ENTERSCORE",

                    // Nhóm quyền mức dộ vai trò
                    "SUPER-ADMIN",
                    "TEACHER",
                    "STUDENT"
                   
                };

                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }

                // Thêm policy TEACHER_OR_STUDENT cho phép có một trong hai quyền
                options.AddPolicy("TEACHER_OR_STUDENT", policy => 
                    policy.RequireAssertion(context => {
                        // Cho phép bất kỳ người dùng đã xác thực
                        if (context.User.Identity?.IsAuthenticated == true)
                        {
                            // Lấy danh sách quyền của người dùng từ claims
                            var permissions = context.User.Claims
                                .Where(c => c.Type == "Permission" || c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

                            // Kiểm tra xem người dùng có quyền TEACHER hoặc STUDENT không
                            return permissions.Contains("TEACHER") || permissions.Contains("STUDENT") || 
                                   context.User.IsInRole("TEACHER") || context.User.IsInRole("STUDENT");
                        }
                        return true; // Tạm thời cho phép tất cả người dùng đã xác thực
                    })
                );
            });

            return services;
        }
    }
}