using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class ClassStudentRepository : IClassStudentRepository
    {
        private readonly ApplicationDbContext _context;
        public ClassStudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ClassStudentRequest request)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId) ?? throw new ArgumentException("Student not found.");
            var classStudent = await _context.Classes.FirstOrDefaultAsync(c => c.Id == request.ClassId) ?? throw new ArgumentException("Class not found.");
            var classAdd = new ClassStudent()
            {
                UserId = request.UserId,
                ClassId = request.ClassId,
                User = student,
                Class = classStudent
            };
            await _context.AddAsync(classAdd);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountByClasses(List<int> ids, string searchTerm = null)
        {
            if (ids == null || !ids.Any())
            {
                return 0;
            }

            var query = _context.ClassStudents
                .Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.IsDelete == false);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(cs =>
                    (cs.User.UserCode != null && cs.User.UserCode.ToLower().Contains(searchTerm)) ||
                    (cs.User.FullName != null && cs.User.FullName.ToLower().Contains(searchTerm)) ||
                    (cs.User.Ethnicity != null && cs.User.Ethnicity.ToLower().Contains(searchTerm)) ||
                    (cs.User.StudentStatus != null && cs.User.StudentStatus.StatusName != null && cs.User.StudentStatus.StatusName.ToLower().Contains(searchTerm)) ||
                    (cs.Class != null && cs.Class.Name != null && cs.Class.Name.ToLower().Contains(searchTerm))
                );
            }

            return await query.CountAsync();
        }

        public async Task<ClassStudent> FindStudentByClassAndStudent(int classId, int studentId)
        {
            return await _context.ClassStudents
                .Include(cs=>cs.Class)
                .ThenInclude(c=>c.ClassType)
                .Include(cs => cs.Class)
                .ThenInclude(c=>c.AcademicYear)
                .Include(cs=>cs.Class)
                .ThenInclude(c=>c.ClassSubjects)
                .Include(cs=>cs.User)
                .Include(cs=>cs.Class)
                .ThenInclude(c=>c.Department)
                .Include(cs=>cs.Class)
                .ThenInclude(c=>c.User)
                .Where(cs=>cs.ClassId == classId && cs.UserId == studentId).FirstOrDefaultAsync();
        }

        public async Task<List<ClassStudent>> GetAllByClasses(List<int> ids, PaginationRequest request, string column, bool orderBy, string searchTerm = null)
        {
            if (ids == null || !ids.Any())
            {
                return new List<ClassStudent>();
            }

            var query = _context.ClassStudents
                .Include(cs => cs.User)
                    .ThenInclude(u => u.StudentStatus)
                .Include(cs => cs.Class)
                .Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.IsDelete == false);
            //Tìm kiếm 
            // Áp dụng tìm kiếm nếu searchTerm không null hoặc rỗng
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower(); // Không phân biệt hoa thường
                query = query.Where(cs =>
                    (cs.User.UserCode != null && cs.User.UserCode.ToLower().Contains(searchTerm)) ||
                    (cs.User.FullName != null && cs.User.FullName.ToLower().Contains(searchTerm)) ||
                    (cs.User.Ethnicity != null && cs.User.Ethnicity.ToLower().Contains(searchTerm)) ||
                    (cs.User.StudentStatus != null && cs.User.StudentStatus.StatusName != null && cs.User.StudentStatus.StatusName.ToLower().Contains(searchTerm)) ||
                    (cs.Class != null && cs.Class.Name != null && cs.Class.Name.ToLower().Contains(searchTerm))
                );
            }
            // Sắp xếp theo cột được chỉ định
            switch (column?.ToLower())
            {
                case "usercode":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.UserCode ?? string.Empty)
                        : query.OrderByDescending(cs => cs.User.UserCode ?? string.Empty);
                    break;

                case "fullname":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.FullName ?? string.Empty)
                        : query.OrderByDescending(cs => cs.User.FullName ?? string.Empty);
                    break;

                case "birthdate":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.BirthDate ?? DateTime.MinValue)
                        : query.OrderByDescending(cs => cs.User.BirthDate ?? DateTime.MinValue);
                    break;

                case "gender":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.Gender)
                        : query.OrderByDescending(cs => cs.User.Gender);
                    break;

                case "ethnicity":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.Ethnicity ?? string.Empty)
                        : query.OrderByDescending(cs => cs.User.Ethnicity ?? string.Empty);
                    break;

                case "status":
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.StudentStatus.StatusName ?? string.Empty)
                        : query.OrderByDescending(cs => cs.User.StudentStatus.StatusName ?? string.Empty);
                    break;

                case "classname":
                    query = orderBy
                        ? query.OrderBy(cs => cs.Class.Name ?? string.Empty)
                        : query.OrderByDescending(cs => cs.Class.Name ?? string.Empty);
                    break;

                default:
                    // Mặc định sắp xếp theo UserCode nếu cột không hợp lệ
                    query = orderBy
                        ? query.OrderBy(cs => cs.User.UserCode ?? string.Empty)
                        : query.OrderByDescending(cs => cs.User.UserCode ?? string.Empty);
                    break;
            }

            if (request != null)
            {
                return await query
                             .Skip((request.PageNumber - 1) * request.PageSize)
                             .Take(request.PageSize)
                             .ToListAsync();
            }
            else
            {
                return await query
                            .ToListAsync();
            }
        }

        public async Task<List<ClassStudent>> getAllStudentByClasses(List<int> ids)
        {
            return await _context.ClassStudents.Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value)).ToListAsync();
        }
    }
}
