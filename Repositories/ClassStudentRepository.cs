using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class ClassStudentRepository : IClassStudentRepository
{
    private readonly ApplicationDbContext _context;

    public ClassStudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public Task<ClassStudent> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ClassStudent>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ClassStudent entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ClassStudent entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}