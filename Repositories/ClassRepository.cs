using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _context;

    public ClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task<Class> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Class>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Class entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Class entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Class>> GetAllClassByAcademicDepartment(int academicId, int departmentId)
    {
        if (academicId > 0) {
            return await _context.Classes.Where(c => c.AcademicYearId == academicId && c.DepartmentId == departmentId && c.IsDelete == false).ToListAsync();
        }
        else
        {
            return await _context.Classes.Where(c => c.AcademicYearId == academicId && c.IsDelete == false).ToListAsync();
        }

    }

    public async Task<Class> FindClassById(int id)
    {
        return await _context.Classes
            .Include(c=>c.TestExams).ThenInclude(te=>te.TestExamType)
            .FirstOrDefaultAsync(c => c.Id == id && c.IsDelete ==false);
    }

    public async Task<List<Class>> GetAllClassByAcademic(int academicId)
    {
        return await _context.Classes.Where(c => c.AcademicYearId == academicId && c.IsDelete == false).ToListAsync();
    }
}