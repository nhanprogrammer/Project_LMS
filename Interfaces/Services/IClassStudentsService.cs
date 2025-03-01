using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IClassStudentsService
    {
        Task<ApiResponse<IEnumerable<ClassStudent>>> GetAllAsync();
        Task<ApiResponse<ClassStudent>> GetByIdAsync(int id);
        Task<ApiResponse<object>> AddAsync(CreateClassStudentRequest request);
        Task<ApiResponse<object>> UpdateAsync(UpdateClassStudentRequest request);
        Task<ApiResponse<object>> DeleteAsync(int id);
    }
}