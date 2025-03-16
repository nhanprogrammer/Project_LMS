using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface ILessonsService
    {
        Task<PaginatedResponse<LessonResponse>> GetLessonAsync(PaginationRequest request);
        Task<ApiResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest createLessonRequest);
        Task<ApiResponse<LessonResponse>> UpdateLessonAsync(UpdateLessonRequest updateLessonRequest);
        Task<ApiResponse<LessonResponse>> DeleteLessonAsync(DeleteRequest deleteRequest);
    }
}