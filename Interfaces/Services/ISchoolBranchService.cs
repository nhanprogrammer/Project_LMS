using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface ISchoolBranchService
    {
        Task<IEnumerable<SchoolBranchResponse>> GetAllAsync();
        Task<SchoolBranchResponse> GetByIdAsync(int id);
        Task<SchoolBranchResponse> CreateAsync(SchoolBranchRequest request, int userId);
        Task<SchoolBranchResponse> UpdateAsync(int id, SchoolBranchRequest request,int userId);
        Task<SchoolBranchResponse> DeleteAsync(int id);
    }
}
