using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class TestExamTypeRepository : ITestExamTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public TestExamTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TestExamType>> GetAllAsync()
        {
            return await _context.TestExamTypes
                .Where(t => t.IsDelete != true)
                .ToListAsync();
        }

        public async Task<TestExamType> GetByIdAsync(int id)
        {
            var testExamType = await _context.TestExamTypes.FindAsync(id);
            if (testExamType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy loại kỳ thi với id {id}.");
            }
            return testExamType;
        }
        
        public async Task AddAsync(TestExamType testExamType)
        {
            await _context.TestExamTypes.AddAsync(testExamType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TestExamType testExamType)
        {
            _context.TestExamTypes.Update(testExamType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TestExamType testExamType)
        {
            _context.TestExamTypes.Remove(testExamType);
            await _context.SaveChangesAsync();
        }
    }
}