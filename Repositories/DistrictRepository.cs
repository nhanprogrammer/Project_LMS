using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class DistrictRepository : IDistrictRepository
{
    private readonly ApplicationDbContext _context;

    public DistrictRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public Task<District> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<District>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(District entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(District entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}