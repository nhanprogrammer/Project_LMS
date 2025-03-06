// Repositories/SubjectsGroupRepository.cs
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class SubjectsGroupRepository : ISubjectsGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectsGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectsGroup>> GetAll(int pageNumber, int pageSize)
        {
            return await _context.SubjectsGroups
                .Where(sg => !(sg.IsDelete ?? false))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SubjectsGroup> GetById(int id)
        {
            return await _context.SubjectsGroups
                .FirstOrDefaultAsync(sg => sg.Id == id && !(sg.IsDelete ?? false));
        }

        public async Task<SubjectsGroup> Add(SubjectsGroup subjectsGroup)
        {
            subjectsGroup.CreateAt = DateTime.UtcNow.ToLocalTime();
            _context.SubjectsGroups.Add(subjectsGroup);
            await _context.SaveChangesAsync();
            return subjectsGroup;
        }

        public async Task<SubjectsGroup> Update(int id, SubjectsGroup subjectsGroup)
        {
            var existing = await _context.SubjectsGroups.FindAsync(id);
            if (existing == null || (existing.IsDelete ?? false))
                return null;

            existing.SubjectId = subjectsGroup.SubjectId;
            existing.Name = subjectsGroup.Name;
            existing.UpdateAt = DateTime.UtcNow.ToLocalTime();
            existing.UserUpdate = subjectsGroup.UserUpdate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var subjectsGroup = await _context.SubjectsGroups.FindAsync(id);
            if (subjectsGroup == null || (subjectsGroup.IsDelete ?? false))
                return false;

            subjectsGroup.IsDelete = true;
            subjectsGroup.UpdateAt = DateTime.UtcNow.ToLocalTime();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}