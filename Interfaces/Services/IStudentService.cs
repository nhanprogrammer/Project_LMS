using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Services;

public interface IStudentService
{
    public Task<ApiResponse<object>> AddAsync(StudentRequest request);
    public Task<ApiResponse<object>> UpdateAsync(StudentRequest request);
    public Task<ApiResponse<object>> DeleteAsync(List<string> userCodes);
    public Task<ApiResponse<StudentResponse>> FindStudentByUserCodeAsync(string userCode);

    public Task<ApiResponse<PaginatedResponse<object>>> GetAllStudentOfRewardOrDisciplines(bool isReward, int academicId, int departmentId, PaginationRequest request, string columnm, bool orderBy, string searchItem);
    public Task<ApiResponse<object>> LearningOutcomesOfStudent(int studentId, int classId);
    public Task<ApiResponse<object>> ExportExcelLearningProcess(int studentId, int classId);

    public Task<List<string>> ValidateStudentRequest(StudentRequest request);
    public Task<string> GeneratedUserCode(string code);
    public Task<string> GeneratedUsername(string email);
    public Task<string> GenerateSecurePassword(int length);
    public Task<ApiResponse<object>> ReadStudentsFromExcelAsync(IFormFile fileExcel);
    public Task ExecuteEmail(string email, string fullname, string username, string password);
    public bool ValidateExcelFormat(Stream fileStream, out string errorMessage);

    public Task<ApiResponse<object>> ExportSampleData();



}