// Repositories/SubjectTypeRepository.cs
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class SubjectTypeRepository : ISubjectTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectType>> GetAll(int pageNumber, int pageSize)
        {
            return await _context.SubjectTypes
                .Where(st => !(st.IsDelete ?? false))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SubjectType?> GetById(int id)
        {
            return await _context.SubjectTypes
                .FirstOrDefaultAsync(st => st.Id == id && !(st.IsDelete ?? false));
        }

        public async Task<SubjectType> Add(SubjectType subjectType)
        {
            subjectType.CreateAt = DateTime.UtcNow.ToLocalTime();
            _context.SubjectTypes.Add(subjectType);
            await _context.SaveChangesAsync();
            return subjectType;
        }

        public async Task<SubjectType?> Update(int id, SubjectType subjectType)
        {
            var existing = await _context.SubjectTypes.FindAsync(id);
            if (existing == null || (existing.IsDelete ?? false))
                return null;

            existing.Name = subjectType.Name;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.UserUpdate = subjectType.UserUpdate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var subjectType = await _context.SubjectTypes.FindAsync(id);
            if (subjectType == null || (subjectType.IsDelete ?? false))
                return false;

            subjectType.IsDelete = true;
            subjectType.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}