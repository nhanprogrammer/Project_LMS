using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AcademicYearRepository : IAcademicYearRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicYearRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AcademicYear> GetByIdAsync(int id)
    {
        return await _context.AcademicYears.FindAsync(id);
    }

    public async Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        return await _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(ah => (bool)!ah.IsDelete)
            .ToListAsync();
    }

    public async Task<AcademicYearWithSemestersDto> GetByIdAcademicYearAsync(int id)
    {
        var academicYear = _context.AcademicYears
            .Where(s => s.Id == id && s.IsDelete == false).FirstOrDefault();
        if (academicYear == null)
        {
            throw new Exception($"Academic Year with ID: {id} not found");
        }

        var semesters = await _context.Semesters
            .Where(s => s.AcademicYearId == id && s.IsDelete == false)
            .Select(se => new SemesterDto
            {
                Id = se.Id,
                Name = se.Name,
                StartDate = se.StartDate.Value,
                EndDate = se.EndDate.Value,
                IsDelete = se.IsDelete.Value
            })
            .ToListAsync();
        var result = new AcademicYearWithSemestersDto
        {
            Id = academicYear.Id,
            StartDate = academicYear.StartDate.Value,
            EndDate = academicYear.EndDate.Value,
            IsInherit = academicYear.IsInherit.Value,
            AcademicParent = academicYear.AcademicParent,
            CreateAt = academicYear.CreateAt.Value,
            UpdateAt = academicYear.UpdateAt.Value,
            UserCreate = academicYear.UserCreate,
            UserUpdate = academicYear.UserUpdate,
            IsDelete = academicYear.IsDelete.Value,
            Semesters = semesters
        };
        return result;
    }

    public async Task AddAsync(AcademicYear academicYear)
    {
        _context.AcademicYears.Add(academicYear);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AcademicYear academicYear)
    {
        _context.AcademicYears.Update(academicYear);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var academicYear = await _context.AcademicYears.FindAsync(id);
        if (academicYear != null)
        {
            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<AcademicYear>> GetByIdsAsync(List<int> ids)
    {
        return await _context.AcademicYears.Where(l => ids.Contains(l.Id)).ToListAsync();
    }

    public async Task UpdateRangeAsync(List<AcademicYear> academicYears)
    {
        _context.AcademicYears.UpdateRange(academicYears);
        await _context.SaveChangesAsync();
    }

    public IQueryable<AcademicYear> GetQueryable()
    {
        return _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(ah => (bool)!ah.IsDelete)
            .AsQueryable();
    }

    public async Task<List<AcademicYear>> SearchAcademicYear(int year)
    {
        return await _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(a => (a.StartDate.HasValue && a.StartDate.Value.Year == year) ||
                        (a.EndDate.HasValue && a.EndDate.Value.Year == year))
            .AsQueryable()
            .ToListAsync();
    }

    public async Task<bool> IsAcademicYearExist(int academicYearId)
    {
        return await _context.AcademicYears.AnyAsync(a => a.Id == academicYearId);
    }
    public async Task<AcademicYear> FindById(int id)
    {
        return await _context.AcademicYears
            .FirstOrDefaultAsync(ay => ay.Id == id && !(ay.IsDelete ?? false))
            ?? throw new InvalidOperationException($"AcademicYear with ID {id} not found.");
    }
}