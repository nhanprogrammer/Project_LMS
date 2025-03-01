using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IClassStudentsOnlineService
    {
        Task<ApiResponse<List<ClassStudentOnlineResponse>>> GetAllClassStudentOnlineAsync();
        Task<ApiResponse<ClassStudentOnlineResponse>> CreateClassStudentOnlineAsync(CreateClassStudentOnlineRequest createClassStudentOnlineRequest);
        Task<ApiResponse<ClassStudentOnlineResponse>> UpdateClassStudentOnlineAsync(string id, UpdateClassStudentOnlineRequest updateClassStudentOnlineRequest);
        Task<ApiResponse<ClassStudentOnlineResponse>> DeleteClassStudentOnlineAsync(string id);
    }
}
