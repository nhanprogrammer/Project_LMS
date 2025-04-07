using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;

namespace Project_LMS.Repositories
{
    public class SemesterRepository : ISemesterRepository
    {
        private readonly ApplicationDbContext _context;

        public SemesterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            return await _context.Semesters
                .AsNoTracking()
                .Where(s => s.IsDelete == false)
                .ToListAsync();
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await _context.Semesters
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.IsDelete == false);
        }

        public async Task AddAsync(Semester semester)
        {
            await _context.Semesters.AddAsync(semester);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Semester semester)
        {
            _context.Semesters.Update(semester);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await _context.Semesters
                .FirstOrDefaultAsync(s => s.Id == id && s.IsDelete == false);
            if (semester != null)
            {
                semester.IsDelete = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddRangeAsync(ICollection<Semester> semesters)
        {
            await _context.Semesters.AddRangeAsync(semesters);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Semester>> GetByAcademicYearIdAsync(int academicYearId)
        {
            return await _context.Semesters
                .Where(s => s.AcademicYearId == academicYearId && s.IsDelete == false)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(List<Semester> semestersToDelete)
        {
            if (semestersToDelete == null || !semestersToDelete.Any())
                return;

            foreach (var semester in semestersToDelete)
            {
                semester.IsDelete = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(List<Semester> updatedSemesters)
        {
            foreach (var semester in updatedSemesters)
            {
                if (_context.Entry(semester).State == EntityState.Detached)
                {
                    _context.Semesters.Attach(semester);
                }
                _context.Entry(semester).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }       
    }
}