using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Services
{
    public interface IClassStudentService

    {
        public Task<ApiResponse<object>> ChangeClassOfStudent(ClassStudentRequest request, int userId);
        public Task<ApiResponse<PaginatedResponse<object>>> GetAllByAcademicAndDepartment(int academicId, int departmentId, PaginationRequest request, string column, bool orderBy,string searchTerm);
        public Task<ApiResponse<object>> ExportAllStudentExcel(int academicId, int departmentId, string column, bool orderBy, string searchItem);
        public Task<ApiResponse<ClassStudentChangeResponse>> GetClassStudentChangeInfo(int userId, int classId);
    }
}
