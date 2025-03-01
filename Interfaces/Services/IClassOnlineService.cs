using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IClassOnlineService
    {
        Task<ApiResponse<List<ClassOnlineResponse>>> GetAllClassOnlineAsync();
        Task<ApiResponse<ClassOnlineResponse>> CreateClassStudentAsync(CreateClassOnlineRequest createClassStudentRequest);
        Task<ApiResponse<ClassOnlineResponse>> UpdateClassStudentAsync(string id, UpdateClassOnlineRequest updateClassStudentRequest);
        Task<ApiResponse<ClassOnlineResponse>> DeleteClassStudentAsync(string id);
    }
}
