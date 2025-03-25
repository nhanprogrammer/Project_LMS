using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public interface IDependentRepository
    {
        public Task<Dependent> AddAsync(Dependent dependent);
        public Task DeleteAsync(Dependent dependent);

    }
}
