using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IDependentService
    {
        public Task<ApiResponse<object>> AddAsync(DependentRequest request);
        public Task<ApiResponse<object>> DeleteAsync(int id);
    }
}
