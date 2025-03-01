using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITestExamTypeService
{
    
    Task<ApiResponse<List<TestExamTypeResponse>>> GetAll();
    Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request);
    Task<ApiResponse<TestExamTypeResponse>> Update(int id, TestExamTypeRequest request);
    Task<ApiResponse<TestExamTypeResponse>> Delete(int id);
    Task<ApiResponse<TestExamTypeResponse>> Search(int id);
    
}