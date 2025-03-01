using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISchoolTransferService
{
    Task<IEnumerable<SchoolTransferResponse>> GetAllAsync();
    Task<SchoolTransferResponse> GetByIdAsync(int id);
    Task<SchoolTransferResponse> CreateAsync(SchoolTransferRequest request);
    Task<SchoolTransferResponse> UpdateAsync(int id, SchoolTransferRequest request);
    Task<SchoolTransferResponse> DeleteAsync(int id);
}