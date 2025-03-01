using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AssignmentDetailRepository : IAssignmentDetailRepository
{
     private readonly ApplicationDbContext _context;

     public AssignmentDetailRepository(ApplicationDbContext context)
     {
         _context = context;
     }

    public async Task<AssignmentDetail> GetByIdAsync(int id)
    {
        return await _context.AssignmentDetails.FindAsync(id);
    }

    public async Task<IEnumerable<AssignmentDetail>> GetAllAsync()
    {
        return await _context.AssignmentDetails.ToListAsync();
    }

    public async Task AddAsync(AssignmentDetail entity)
    {
        await _context.AssignmentDetails.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AssignmentDetail entity)
    {

        _context.AssignmentDetails.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.AssignmentDetails.FindAsync(id);
        if (assignment != null)
        {
            _context.AssignmentDetails.Remove(assignment);
            await _context.SaveChangesAsync();
        }

    }
}