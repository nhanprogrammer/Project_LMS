
using Project_LMS.DTOs.Response;


namespace Project_LMS.Interfaces.Services
{
    public interface IStudentStatusService
    {
        public Task<ApiResponse<List<StudentStatusResponse>>> GetAll();
        public Task<ApiResponse<StudentStatusResponse>> Search(int id);

    }
}
