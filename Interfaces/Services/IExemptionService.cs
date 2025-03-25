using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
namespace Project_LMS.Interfaces.Services
{
    public interface IExemptionService
    {
        public Task<ApiResponse<object>> AddAsync(ExemptionRequest request);
    }
}
