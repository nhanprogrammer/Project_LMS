using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class TeachingAssignmentRepository :ITeachingAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TeachingAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TeachingAssignment>> GetAllByClasses(List<int> classIds)
        {
            return await _context.TeachingAssignments.Where(t=>classIds.Contains((int)t.ClassId) && t.IsDelete == false && t.User.IsDelete ==false).ToListAsync();
        }
    }
}
