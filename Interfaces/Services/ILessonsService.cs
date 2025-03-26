using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface ILessonService
    {
        Task<ApiResponse<PaginatedResponse<LessonResponse>>> GetLessonAsync(string? keyword, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<LessonResponse>> GetLessonByIdAsync(int id);
        Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest request);
        Task<ApiResponse<LessonResponse>> UpdateLessonAsync(CreateLessonRequest request);
        Task<ApiResponse<bool>> DeleteLessonAsync(int id);
        Task<ApiResponse<bool>> DeleteMultipleLessonsAsync(List<int> ids);
    }
}