using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ITestExamTypeService
{
    
    Task<ApiResponse<PaginatedResponse<TestExamTypeResponse>>> GetAll(int pageNumber = 1, int pageSize = 10,string? keyword = null);
    Task<ApiResponse<TestExamTypeResponse>> Create(TestExamTypeRequest request, int userId);
    Task<ApiResponse<TestExamTypeResponse>> Update(int id, TestExamTypeRequest request, int userId);
    Task<ApiResponse<TestExamTypeResponse>> Delete(int id);
    Task<ApiResponse<TestExamTypeResponse>> Search(int id);
    Task<ApiResponse<List<int>>> GetCoefficients();
    Task<ApiResponse<TestExamTypeResponse>> GetById(int id);
    Task<List<DropdownTestExamTypeResponse>> GetDropdown();
}