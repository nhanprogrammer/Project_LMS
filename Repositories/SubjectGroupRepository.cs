using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_LMS.Interfaces.Responsitories;


namespace Project_LMS.Repositories
{
    public class SubjectGroupRepository :ISubjectGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubjectGroup> GetByIdAsync(int id)
        {
            return await _context.SubjectGroups
                .Include(sg => sg.SubjectGroupSubjects)  
                .ThenInclude(sgs => sgs.Subject)
                // .Include(sg=>sg.User)
                .FirstOrDefaultAsync(sg => sg.Id == id) ?? throw new InvalidOperationException("SubjectGroup not found");
        }
        
      
        
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SubjectGroup>> GetAllAsync()
        {
            return await _context.SubjectGroups
                .Where(sg => sg.IsDelete == false)
                .Include(sg => sg.SubjectGroupSubjects)
                .ThenInclude(sgs => sgs.Subject)
                // .Include(sg => sg.User) // Chỉ Include nếu thực sự cần thông tin User
                .ToListAsync();
        }

        public async Task AddAsync(SubjectGroup entity)
        {
            await _context.SubjectGroups.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubjectGroup entity)
        {
            _context.SubjectGroups.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.SubjectGroups.FindAsync(id);
            if (entity != null)
            {
                entity.IsDelete = true;
                _context.SubjectGroups.Update(entity);
                var relatedSubjects = _context.SubjectGroupSubjects.Where(sgs => sgs.SubjectGroupId == id);
                _context.SubjectGroupSubjects.RemoveRange(relatedSubjects);

                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<SubjectGroup> GetQueryable()
        {
            return _context.SubjectGroups.AsQueryable();
        }


    }
}