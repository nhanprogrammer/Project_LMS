using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IDisciplinesService
    {
        Task<ApiResponse<object>> GetByIdAsync(int id);
        Task<ApiResponse<object>> AddAsync(DisciplineRequest request);
        Task<ApiResponse<object>> UpdateAsync(UpdateDisciplineRequest request);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}