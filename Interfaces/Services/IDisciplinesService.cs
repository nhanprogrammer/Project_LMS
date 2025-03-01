using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IDisciplinesService
    {
        Task<ApiResponse<List<DisciplineResponse>>> GetAllDisciplineAsync();
        Task<ApiResponse<DisciplineResponse>> CreateDisciplineAsync(CreateDisciplineRequest createDisciplineRequest);
        Task<ApiResponse<DisciplineResponse>> UpdateDisciplineAsync(string id, UpdateDisciplineRequest updateDisciplineRequest);
        Task<ApiResponse<DisciplineResponse>> DeleteDisciplineAsync(string id);
    }
}