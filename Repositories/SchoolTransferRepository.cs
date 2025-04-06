using Microsoft.EntityFrameworkCore;
using Project_LMS.Models;
using Project_LMS.Data;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;

namespace Project_LMS.Repositories
{
    public class SchoolTransferRepository : ISchoolTransferRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolTransferRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SchoolTransfer>> GetAllAsync(int id, PaginationRequest request, bool isOrder, string column, string searchItem)
        {
            var query = _context.SchoolTransfers
                .Include(s => s.User).ThenInclude(u => u.ClassStudents).ThenInclude(cs => cs.Class).ThenInclude(c => c.Department)
                .Where(s => s.User.ClassStudents.Where(cs => cs.IsActive == true && cs.IsDelete == false).FirstOrDefault().Class.AcademicYearId == id);
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.Trim().ToLower();
                query = query.Where(t =>
                (t.User.UserCode != null && t.User.UserCode.ToLower().Contains(searchItem)) ||
                (t.User.FullName != null && t.User.FullName.ToLower().Contains(searchItem)) ||
                (t.Semester != null && t.Semester.ToLower().Contains(searchItem))
                );
            }
            switch (column.ToLower())
            {
                case "usercode":
                    query = isOrder ? query.OrderBy(s => s.User.UserCode)
                            : query.OrderByDescending(s => s.User.UserCode);
                    break;
                case "fullname":
                    query = isOrder ? query.OrderBy(s => s.User.FullName)
                            : query.OrderByDescending(s => s.User.FullName);
                    break;
                case "birthdate":
                    query = isOrder ? query.OrderBy(s => s.User.BirthDate)
                            : query.OrderByDescending(s => s.User.BirthDate);
                    break;
                case "gender":
                    query = isOrder ? query.OrderBy(s => s.User.Gender)
                            : query.OrderByDescending(s => s.User.Gender);
                    break;
                case "transferfrom":
                    query = isOrder ? query.OrderBy(s => s.TransferFrom)
                            : query.OrderByDescending(s => s.TransferFrom);
                    break;
                case "semester":
                    query = isOrder ? query.OrderBy(s => s.Semester)
                            : query.OrderByDescending(s => s.Semester);
                    break;
                case "department":
                    query = isOrder ? query.OrderBy(s => s.User.ClassStudents.Where(cs => cs.IsActive == true && cs.IsDelete == false).FirstOrDefault().Class.Department.Name)
                            : query.OrderByDescending(s => s.User.ClassStudents.Where(cs => cs.IsActive == true && cs.IsDelete == false).FirstOrDefault().Class.Department.Name);
                    break;
                case "transferdate":
                    query = isOrder ? query.OrderBy(s => s.TransferDate)
                            : query.OrderByDescending(s => s.TransferDate);
                    break;
                default:
                    query = isOrder ? query.OrderBy(s => s.User.UserCode)
                           : query.OrderByDescending(s => s.User.UserCode);
                    break;

            }
            return await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<SchoolTransfer?> GetByIdAsync(int id)
        {
            return await _context.SchoolTransfers.FindAsync(id);
        }

        public async Task AddAsync(SchoolTransfer schoolTransfer)
        {
            await _context.SchoolTransfers.AddAsync(schoolTransfer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SchoolTransfer schoolTransfer)
        {
            _context.SchoolTransfers.Update(schoolTransfer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var schoolTransfer = await _context.SchoolTransfers.FindAsync(id);
            if (schoolTransfer != null)
            {
                _context.SchoolTransfers.Remove(schoolTransfer);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<SchoolTransfer> GetByStudentId(int studentId)
        {
            return await _context.SchoolTransfers
                .Where(st => st.UserId == studentId && (st.IsDelete.HasValue && !st.IsDelete.Value))
                .OrderByDescending(st => st.CreateAt)
                .FirstOrDefaultAsync();
        }
    }
}