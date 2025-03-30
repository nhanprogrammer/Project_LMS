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
            // Lấy tất cả các bản ghi ClassStudent có UserId và IsActive = true
            var activeClasses = await _context.ClassStudents
                .Where(cs => cs.UserId.HasValue && cs.UserId.Value == request.UserId && cs.ClassId != request.ClassId && cs.IsActive == true  && cs.IsDelete == false )
                .FirstOrDefaultAsync();

            if (activeClasses != null)
            {
                activeClasses.IsActive = false;
                if (activeClasses.IsClassTransitionStatus == false)
                {
                    activeClasses.IsClassTransitionStatus = true;
                }
                _context.ClassStudents.UpdateRange(activeClasses);

                // Lưu thay đổi
                await _context.SaveChangesAsync();
            }

            var classStudent = await _context.ClassStudents.FirstOrDefaultAsync(cs =>cs.UserId == request.UserId && cs.ClassId == request.ClassId && cs.IsDelete == false);
            if (classStudent != null) {
                classStudent.IsDelete = false;
                classStudent.IsActive = true;
                classStudent.IsClassTransitionStatus = false;
            } else
            {
                var classAdd = new ClassStudent()
                {
                    UserId = request.UserId,
                    ClassId = request.ClassId,
                    //User = student,
                    //Class = classStudent,
                    IsDelete = false,
                    IsActive = true,
                    IsClassTransitionStatus = false
                };

                await _context.ClassStudents.AddAsync(classAdd);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<int> CountByClasses(List<int> ids, string searchTerm = null)
        {
            if (ids == null || !ids.Any())
            {
                return 0;
            }

            var query = _context.ClassStudents
                .Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.IsDelete == false && cs.User.IsDelete ==false);

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

        public async Task<ClassStudent> FindClassStudentByUserCodeClassId(string userCode, int classId)
        {
            return await _context.ClassStudents.FirstOrDefaultAsync(cs => cs.User.UserCode == userCode && cs.ClassId == classId && cs.Class.IsDelete ==false && cs.IsDelete == false && cs.User.IsDelete ==false);
        }

        public async Task<ClassStudent> FindStudentByClassAndStudent(int classId, int studentId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .ThenInclude(c => c.ClassType)
                .Include(cs => cs.Class)
                .ThenInclude(c => c.AcademicYear)
                .Include(cs => cs.Class)
                .ThenInclude(c => c.ClassSubjects)
                .Include(cs => cs.User)
                .Include(cs => cs.Class)
                .ThenInclude(c => c.Department)
                .Include(cs => cs.Class)
                .ThenInclude(c => c.User)
                .Where(cs => cs.ClassId == classId && cs.UserId == studentId && cs.Class.IsDelete == false && cs.IsDelete == false && cs.User.IsDelete == false).FirstOrDefaultAsync();
        }

        public async Task<ClassStudent> FindStudentByIdIsActive(int studentId)
        {
            return await _context.ClassStudents
                .Include(cs=>cs.User)
                .Include(cs=>cs.Class)
                .FirstOrDefaultAsync(cs => cs.UserId == studentId && cs.IsActive == true && cs.Class.IsDelete == false && cs.IsDelete == false && cs.User.IsDelete == false);
        }

        public async Task<List<ClassStudent>> FindStudentByStudentAcademic(int studentId, int academicId)
        {
            return await _context.ClassStudents
                 .Include(cs => cs.User).ThenInclude(u => u.Assignments).ThenInclude(asm => asm.TestExam).ThenInclude(te => te.TestExamType)
                 .Include(cs => cs.Class)
                 .Include(cs => cs.Class).ThenInclude(c => c.AcademicYear)
                 .Include(cs => cs.Class).ThenInclude(c => c.Department)
                 .Include(cs => cs.Class).ThenInclude(c => c.User)
                 .Include(cs => cs.Class).ThenInclude(c => c.ClassSubjects).ThenInclude(cs => cs.Subject)
                 .Include(cs => cs.Class).ThenInclude(c => c.TeachingAssignments).ThenInclude(t => t.User)
                 .Where(cs => cs.UserId == studentId && cs.IsDelete == false && cs.Class.IsDelete == false).ToListAsync();
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
                .Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.Class.IsDelete == false && cs.IsDelete == false && cs.User.IsDelete == false);
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
            return await _context.ClassStudents.Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.Class.IsDelete == false && cs.IsDelete == false && cs.User.IsDelete == false).ToListAsync();
        }

        public async Task UpdateClassIdAsync(int studentId, int classId)
        {
            var classStudent = await _context.ClassStudents.FirstOrDefaultAsync(cs => cs.UserId == studentId && cs.IsActive == true);
            classStudent.ClassId = classId;
            _context.ClassStudents.Update(classStudent);
            await _context.SaveChangesAsync();
        }
    }
}
