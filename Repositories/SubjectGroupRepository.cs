using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Repositories
{
    public class SubjectGroupRepository : ISubjectGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubjectsGroup> GetByIdAsync(int id)
        {
            return await _context.SubjectsGroups
                .Include(sg => sg.User)
                .Include(sg => sg.SubjectGroupSubjects)  
                .ThenInclude(sgs => sgs.Subject)  
                .FirstOrDefaultAsync(sg => sg.Id == id) ?? throw new InvalidOperationException("SubjectGroup not found");
        }
        
      
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubjectsGroup>> GetAllAsync()
        {
            return await _context.SubjectsGroups.Where(sg => sg.IsDelete == false)
                .Include(sg => sg.SubjectGroupSubjects).ThenInclude(sgs => sgs.Subject).Include(sg => sg.User)
                .ToListAsync();
        }

        public async Task AddAsync(SubjectsGroup entity)
        {
            await _context.SubjectsGroups.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubjectsGroup entity)
        {
            _context.SubjectsGroups.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.SubjectsGroups.FindAsync(id);
            if (entity != null)
            {
                entity.IsDelete = true;
                _context.SubjectsGroups.Update(entity);
                var relatedSubjects = _context.SubjectGroupsSubjects.Where(sgs => sgs.SubjectGroupId == id);
                _context.SubjectGroupsSubjects.RemoveRange(relatedSubjects);

                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<SubjectsGroup> GetQueryable()
        {
            return _context.SubjectsGroups.AsQueryable();
        }


    }
}