using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IRegistrationContactsService
    {
        Task<IEnumerable<RegistrationContactResponse>> GetAllAsync();
        Task<RegistrationContactResponse> GetByIdAsync(int id);
        Task<RegistrationContactResponse> CreateAsync(RegistrationContactRequest request);
        Task<RegistrationContactResponse> UpdateAsync(int id, RegistrationContactRequest request);
        Task<RegistrationContactResponse> DeleteAsync(int id);
    }
}