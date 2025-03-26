using Microsoft.AspNetCore.Authorization;

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
                    // Quản lý lớp học Teacher
                    "TEACHER-CLASS-FUTURE-VIEW",
                    "TEACHER-CLASS-FUTURE-DETAIL-VIEW",

                    "STUDENT-CLASS-FUTURE-VIEW",
                    "STUDENT-CLASS-FUTURE-DETAIL-VIEW",

                    "LESSON-VIEW",
                    "LESSON-INSERT",
                    "LESSON-UPDATE",
                    "LESSON-DELETE",
                    "LESSON-ENTERSCORE",

                    "SUBJECT-VIEW",
                    "SUBJECT-INSERT",
                    "SUBJECT-UPDATE",
                    "SUBJECT-DELETE",
                    "SUBJECT-ENTERSCORE",

                    "SUBJECT-TYPE-VIEW",
                    "SUBJECT-TYPE-INSERT",
                    "SUBJECT-TYPE-UPDATE",
                    "SUBJECT-TYPE-DELETE",
                    "SUBJECT-TYPE-ENTERSCORE",

                    "CLASS-TYPE-VIEW",
                    "CLASS-TYPE-INSERT",
                    "CLASS-TYPE-UPDATE",
                    "CLASS-TYPE-DELETE",
                    "CLASS-TYPE-ENTERSCORE"
                };

                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            return services;
        }
    }

}