using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{

    public class ClassSubjectRepository : IClassSubjectRepository
    {
        private readonly ApplicationDbContext _context;
        public ClassSubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassSubject>> GetAllByClass(int classId)
        {
        return  _context.ClassSubjects
                .Include(x => x.Class)
                .Include(x=>x.Subject)
                .Where(cs => cs.ClassId == classId).ToList(); ;
        }
    }
}
