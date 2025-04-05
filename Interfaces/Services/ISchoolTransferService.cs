using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services;

public interface ISchoolTransferService
{
    Task<ApiResponse<List<object>>> GetAllAsync(int academicId, PaginationRequest request, bool isOrder, string column, string searchItem);
    Task<SchoolTransferResponse> GetByIdAsync(int id);
    Task<SchoolTransferResponse> CreateAsync(SchoolTransferRequest request, int StudentId);
    Task<SchoolTransferResponse> UpdateAsync(int id, SchoolTransferRequest request);
    Task<SchoolTransferResponse> DeleteAsync(int id);
}