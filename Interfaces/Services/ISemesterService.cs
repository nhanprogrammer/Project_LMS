using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISemesterService
{
    Task<IEnumerable<SemesterResponse>> GetAllAsync();
    Task<SemesterResponse> GetByIdAsync(int id);
    Task<SemesterResponse> CreateAsync(SemesterRequest request);
    Task<SemesterResponse> UpdateAsync(int id, SemesterRequest request);
    Task<SemesterResponse> DeleteAsync(int id);
}