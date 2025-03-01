namespace Project_LMS.Interfaces.Services;

using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

public interface ISchoolService
{
    Task<IEnumerable<SchoolResponse>> GetAllAsync();
    Task<SchoolResponse> GetByIdAsync(int id);
    Task<SchoolResponse> CreateAsync(SchoolRequest request);
    Task<SchoolResponse> UpdateAsync(int id, SchoolRequest request);
    Task<SchoolResponse> DeleteAsync(int id);
}
