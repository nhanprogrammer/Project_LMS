using Project_LMS.Models;

namespace Project_LMS.Interfaces.Repositories
{
    public interface ITestExamTypeRepository
    {
        Task<List<TestExamType>> GetAllAsync();
        Task<TestExamType> GetByIdAsync(int id);
        Task AddAsync(TestExamType testExamType);
        Task UpdateAsync(TestExamType testExamType);
        Task DeleteAsync(TestExamType testExamType);
    }
}