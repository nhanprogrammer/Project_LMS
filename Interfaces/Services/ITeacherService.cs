using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

using Project_LMS.Models;
namespace Project_LMS.Interfaces.Services;

public interface ITeacherService
{
    public Task<ApiResponse<object>> FindTeacherByUserCode(string userCode);
    public Task<ApiResponse<object>> AddAsync(TeacherRequest request);
    public Task<ApiResponse<object>> UpdateAsync(TeacherRequest request);
    public Task<ApiResponse<object>> DeleteAsync(List<string> userCodes);
    public Task ExecuteEmail(string email, string fullname, string username, string password);
    public Task<ApiResponse<PaginatedResponse<object>>> GetAllByAcademic(int acadimicId, PaginationRequest request, bool orderBy, string column, string searchItem);
    public Task<ApiResponse<object>> ExportExcelByAcademic(int acadimicId, bool orderBy, string column, string searchItem);
    Task<List<UserResponseTeachingAssignment>> GetTeachersAsync();
    //public Task<ApiResponse<object>> DropdownDepartmentTeacher();

}