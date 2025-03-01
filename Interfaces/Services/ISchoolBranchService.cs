using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ISchoolBranchService
    {
        Task<IEnumerable<SchoolBranchResponse>> GetAllAsync();
        Task<SchoolBranchResponse> GetByIdAsync(int id);
        Task<SchoolBranchResponse> CreateAsync(SchoolBranchRequest request);
        Task<SchoolBranchResponse> UpdateAsync(int id, SchoolBranchRequest request);
        Task<SchoolBranchResponse> DeleteAsync(int id);
    }
}
