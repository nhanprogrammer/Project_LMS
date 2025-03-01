using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IRegistrationContactRepository
    {
        Task<IEnumerable<RegistrationContact>> GetAllAsync();
        Task<RegistrationContact?> GetByIdAsync(int id);
        Task AddAsync(RegistrationContact registrationContact);
        Task UpdateAsync(RegistrationContact registrationContact);
        Task DeleteAsync(int id);
    }
}