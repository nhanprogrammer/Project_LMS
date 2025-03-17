using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
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

    public async Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        return await _context.AcademicYears.ToListAsync();
    }

    public async Task<AcademicYear> GetByIdAsync(int id)
    {
        return await _context.AcademicYears.FindAsync(id);
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
        return _context.AcademicYears.AsQueryable();
    }

    public async Task InheritAcademicYear(int newYearId, int oldYearId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // ✅ 1. Kế thừa danh sách lớp học
            var oldClasses = await _context.Classes.Where(c => c.AcademicYearId == oldYearId).ToListAsync();
            var newClasses = oldClasses.Select(c => new Class
            {
                Name = c.Name,
                AcademicYearId = newYearId,
                CreateAt = DateTime.UtcNow // Changed from CreatedAt to CreateAt
            }).ToList();
            await _context.Classes.AddRangeAsync(newClasses);
            await _context.SaveChangesAsync();

            // ✅ 2. Kế thừa danh sách học viên
            var oldStudents = await _context.Students.Where(s => s.AcademicYearId == oldYearId).ToListAsync();
            var newStudents = oldStudents.Select(s => new Student
            {
                Name = s.Name,
                ClassId = newClasses.FirstOrDefault(c => c.Name == s.Class.Name)?.Id,
                AcademicYearId = newYearId,
                CreatedAt = DateTime.UtcNow
            }).ToList();
            await _context.Students.AddRangeAsync(newStudents);
            await _context.SaveChangesAsync();

            // ✅ 3. Kế thừa môn học
            var oldSubjects = await _context.Subjects.Where(s => s.AcademicYearId == oldYearId).ToListAsync();
            var newSubjects = oldSubjects.Select(s => new Subject
            {
                Name = s.Name,
                Credit = s.Credit,
                AcademicYearId = newYearId,
                CreatedAt = DateTime.UtcNow
            }).ToList();
            await _context.Subjects.AddRangeAsync(newSubjects);
            await _context.SaveChangesAsync();

            // ✅ 4. Kế thừa phân công giảng dạy
            var oldAssignments = await _context.TeachingAssignments.Where(t => t.AcademicYearId == oldYearId).ToListAsync();
            var newAssignments = oldAssignments.Select(t => new TeachingAssignment
            {
                TeacherId = t.TeacherId,
                SubjectId = newSubjects.FirstOrDefault(s => s.Name == t.Subject.Name)?.Id,
                ClassId = newClasses.FirstOrDefault(c => c.Name == t.Class.Name)?.Id,
                AcademicYearId = newYearId,
                CreatedAt = DateTime.UtcNow
            }).ToList();
            await _context.TeachingAssignments.AddRangeAsync(newAssignments);
            await _context.SaveChangesAsync();

            // Hoàn thành giao dịch
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}