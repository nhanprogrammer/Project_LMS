using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRegistrationService
    {
        Task<IEnumerable<RegistrationResponse>> GetAllAsync();
        Task<RegistrationResponse> GetByIdAsync(int id);
        Task<RegistrationResponse> CreateAsync(RegistrationRequest request);
        Task<RegistrationResponse> UpdateAsync(int id, RegistrationRequest request);
        Task<RegistrationResponse> DeleteAsync(int id);
    }
}