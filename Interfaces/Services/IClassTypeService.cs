using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces
{
    public interface IClassTypeService
    {
        Task<ApiResponse<IEnumerable<ClassType>>> GetAllAsync();  // Lấy tất cả ClassTypes
        Task<ApiResponse<ClassType>> GetByIdAsync(int id);  // Lấy ClassType theo ID
        Task<ApiResponse<object>> AddAsync(ClassType classType);  // Thêm ClassType mới
        Task<ApiResponse<object>> UpdateAsync(ClassType classType);  // Cập nhật ClassType
        Task<ApiResponse<object>> DeleteAsync(int id);  // Xóa ClassType
    }
}