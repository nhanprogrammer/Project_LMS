﻿using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ITestExamScheduleService
    {
        Task<ApiResponse<List<TestExamScheduleResponse>>> GetExamScheduleAsync(int? month,int? year, bool week,int? departmentid, DateTimeOffset? startDateOffWeek);

        Task<ApiResponse<List<TestExamScheduleDetailResponse>>> GetExamScheduleDetailAsync(
            DateTimeOffset startdate);

        Task<ApiResponse<List<TestExamScheduleDetailForStudentAndTeacherResponse>>>
            GetExamScheduleDetailForStudentAndTeacherAsync(
                DateTimeOffset startdate);


         Task<ApiResponse<Object>> DeleteExamScheduleDetailByIdAsync(int id);
    }
}