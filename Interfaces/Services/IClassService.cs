using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IClassService
    {
        Task<ApiResponse<IEnumerable<Class>>> GetAllClassesAsync();
        Task<ApiResponse<Class>> GetClassByIdAsync(int id);
        //Task<ApiResponse<Class>> CreateClassAsync(CreateClassRequest request);
        //Task<ApiResponse<Class>> UpdateClassAsync(int id, UpdateClassRequest request);


        Task<ClassDto> UpdateClassAsync(int id, CreateClassRequest request);

        Task<ClassDto> CreateClassAsync(CreateClassRequest request);
        Task<ApiResponse<Class>> UpdateClassAsync(int id, UpdateClassRequest request);  // <- Cần kiểm tra
        Task<ApiResponse<bool>> DeleteClassAsync(int id);  // Cần có
    }
}


// note nè Tỷ