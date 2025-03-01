using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITestExamService
{
    
    Task<ApiResponse<List<TestExamResponse>>> GetAll();
    Task<ApiResponse<TestExamResponse>> Create(TestExamRequest request);
    Task<ApiResponse<TestExamResponse>> Update(int id, TestExamRequest request);
    Task<ApiResponse<TestExamResponse>> Delete(int id);
    Task<ApiResponse<TestExamResponse>> Search(int id);
    
}