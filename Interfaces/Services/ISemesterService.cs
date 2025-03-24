using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Services;

public interface ISemesterService
{
    Task<IEnumerable<SemesterResponse>> GetAllAsync();
    Task<SemesterResponse> GetByIdAsync(int id);
    Task<ApiResponse<SemesterResponse>> CreateSemesters(List<CreateSemesterRequest> request, int academicYearId, int userId);
    Task<ApiResponse<SemesterResponse>> UpdateSemesters(List<UpdateSemesterRequest> semesters, int academicYearId, int userId);
    Task<SemesterResponse> DeleteAsync(int id);
}