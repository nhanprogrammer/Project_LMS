using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AcademicHoldRepository : IAcademicHoldRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicHoldRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicHold> GetByIdAsync(int id)
    {
        return await _context.AcademicHolds
             .Include(a => a.User)
            //  .ThenInclude(u => u.Students)
            //  .ThenInclude(s => s.ClassStudents)
            //  .ThenInclude(cs => cs.Class)
            //  .Include(u => u.User.Students)
            //  .ThenInclude(s => s.AcademicYear)
            //  .ThenInclude(ay => ay.Semesters)
             .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AcademicHold>> GetAllAsync()
    {
        return await _context.AcademicHolds
            .Include(a => a.User)
            // .ThenInclude(u => u.Students)
            // .ThenInclude(s => s.ClassStudents)
            // .ThenInclude(cs => cs.Class)
            // .Include(u => u.User.Students)
            // .ThenInclude(s => s.AcademicYear)
            // .ThenInclude(ay => ay.Semesters)
            .ToListAsync();
    }

    public async Task AddAsync(AcademicHold entity)
    {
        await _context.AcademicHolds.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AcademicHold entity)
    {
        _context.AcademicHolds.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.AcademicHolds.FindAsync(id);
        if (entity != null)
        {
            entity.IsDelete = true;
            _context.AcademicHolds.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<AcademicHold> GetByStudentIdAsync(int studentId)
    {
        return await _context.AcademicHolds
            .FirstOrDefaultAsync(ah => ah.UserId == studentId);  // Sử dụng StudentId để tìm kiếm
    }

    public IQueryable<AcademicHold> GetQueryable()
    {
        return _context.AcademicHolds.AsQueryable();
    }

}