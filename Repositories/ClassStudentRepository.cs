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
        private readonly ILogger<ClassStudentRepository> _logger;
        public ClassStudentRepository(ApplicationDbContext context, ILogger<ClassStudentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddChangeClassAsync(ClassStudentRequest request)
        {
            var newClass = await _context.Classes
                .Include(c => c.AcademicYear)
                .FirstOrDefaultAsync(c => c.Id == request.ClassId);
            if (newClass == null || newClass.AcademicYear == null || !newClass.AcademicYear.StartDate.HasValue)
            {
                throw new Exception("Lớp học hoặc niên khóa không hợp lệ.");
            }

            // Lấy tất cả các bản ghi ClassStudent đang active của học sinh
            var activeClasses = await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.AcademicYear)
                .Where(cs => cs.UserId == request.UserId
                    && cs.IsActive == true
                    && cs.IsDelete == false
                    && cs.ClassId != request.ClassId) // Không lấy lớp đang chuyển đến
                .ToListAsync();

            if (activeClasses.Any())
            {
                // Cập nhật trạng thái các lớp cũ
                foreach (var activeClass in activeClasses)
                {
                    activeClass.IsActive = false;
                    activeClass.IsClassTransitionStatus = true;
                    activeClass.UserUpdate = request.UserUpdate;
                    activeClass.UpdateAt = DateTime.Now;
                }
                _context.ClassStudents.UpdateRange(activeClasses);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem đã có bản ghi ClassStudent với lớp mới chưa
            var existingClassStudent = await _context.ClassStudents
                .FirstOrDefaultAsync(cs => cs.UserId == request.UserId
                    && cs.ClassId == request.ClassId
                    && cs.IsDelete == false);

            if (existingClassStudent != null)
            {
                // Nếu đã có bản ghi, cập nhật trạng thái
                existingClassStudent.IsDelete = false;
                existingClassStudent.IsActive = true;
                existingClassStudent.IsClassTransitionStatus = false;
                existingClassStudent.UserUpdate = request.UserUpdate;
                existingClassStudent.UpdateAt = DateTime.Now;
                existingClassStudent.Reason = request.Reason;
                existingClassStudent.FileName = request.FileName;
                existingClassStudent.ChangeDate = request.ChangeDate;

                _context.ClassStudents.Update(existingClassStudent);
            }
            else
            {
                // Tạo bản ghi mới
                var newClassStudent = new ClassStudent
                {
                    UserId = request.UserId,
                    ClassId = request.ClassId,
                    IsDelete = false,
                    IsActive = true,
                    IsClassTransitionStatus = false,
                    Reason = request.Reason,
                    FileName = request.FileName,
                    ChangeDate = request.ChangeDate,
                    UserCreate = request.UserCreate,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };
                await _context.ClassStudents.AddAsync(newClassStudent);
            }

            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(ClassStudentRequest request)
        {
            // Lấy thông tin lớp và niên khóa của bản ghi mới
            var newClass = await _context.Classes
                .Include(c => c.AcademicYear)
                .FirstOrDefaultAsync(c => c.Id == request.ClassId);
            if (newClass == null || newClass.AcademicYear == null || !newClass.AcademicYear.StartDate.HasValue)
            {
                throw new Exception("Lớp học hoặc niên khóa không hợp lệ.");
            }
            var newAcademicYearStartDate = newClass.AcademicYear.StartDate.Value;

            // Lấy tất cả các bản ghi ClassStudent có UserId và IsActive = true
            var activeClasses = await _context.ClassStudents
                .Include(cs => cs.Class)
                .ThenInclude(c => c.AcademicYear)
                .Where(cs => cs.UserId.HasValue && cs.UserId.Value == request.UserId && cs.ClassId != request.ClassId && cs.IsActive == true && cs.IsDelete == false)
                .ToListAsync(); // Sử dụng ToListAsync để lấy tất cả bản ghi

            if (activeClasses != null && activeClasses.Any())
            {
                foreach (var activeClass in activeClasses)
                {
                    // Kiểm tra niên khóa của bản ghi hiện tại
                    if (activeClass.Class != null && activeClass.Class.AcademicYear != null &&
                        activeClass.Class.AcademicYear.StartDate.HasValue &&
                        activeClass.Class.AcademicYear.StartDate.Value > newAcademicYearStartDate)
                    {
                        activeClass.IsActive = false;
                        if (activeClass.IsClassTransitionStatus == false)
                        {
                            activeClass.IsClassTransitionStatus = true;
                        }
                        _logger.LogInformation("Đánh dấu bản ghi ClassStudent không hoạt động do thêm bản ghi mới: UserId={UserId}, ClassId={ClassId}, SchoolYear={SchoolYear}",
                            activeClass.UserId, activeClass.ClassId, activeClass.Class.AcademicYearId);
                    }
                    else
                    {
                        _logger.LogInformation("Bỏ qua bản ghi ClassStudent vì niên khóa không thỏa mãn: UserId={UserId}, ClassId={ClassId}, SchoolYear={SchoolYear}",
                            activeClass.UserId, activeClass.ClassId, activeClass.Class?.AcademicYearId);
                    }
                }
                _context.ClassStudents.UpdateRange(activeClasses);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem đã có bản ghi ClassStudent với UserId và ClassId chưa
            var classStudent = await _context.ClassStudents
                .FirstOrDefaultAsync(cs => cs.UserId == request.UserId && cs.ClassId == request.ClassId && cs.IsDelete == false);
            if (classStudent != null)
            {
                classStudent.IsDelete = false;
                classStudent.IsActive = true;
                classStudent.IsClassTransitionStatus = false;
                _context.ClassStudents.Update(classStudent);
            }
            else
            {
                var classAdd = new ClassStudent()
                {
                    UserId = request.UserId,
                    ClassId = request.ClassId,
                    IsDelete = false,
                    IsActive = true,
                    IsClassTransitionStatus = false,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };
                await _context.ClassStudents.AddAsync(classAdd);
            }
            await _context.SaveChangesAsync();
        }


        public async Task<int> CountByClasses(List<int> ids, string searchTerm = null)
        {
            if (ids == null || !ids.Any())
            {
                return 0;
            }

            var query = _context.ClassStudents
                .Where(cs => cs.ClassId.HasValue && ids.Contains(cs.ClassId.Value) && cs.IsDelete == false && cs.User.IsDelete == false);

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
            return await _context.ClassStudents.FirstOrDefaultAsync(cs => cs.User.UserCode == userCode && cs.ClassId == classId && cs.Class.IsDelete == false && cs.IsDelete == false && cs.User.IsDelete == false);
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
                .Include(cs => cs.User)
                .Include(cs => cs.Class)
                .ThenInclude(c => c.AcademicYear)
                .Where(cs => cs.UserId == studentId
                    && cs.IsActive == true
                    && cs.IsDelete == false
                    && cs.User.IsDelete == false
                    && cs.Class != null
                    && cs.Class.IsDelete == false
                    && cs.Class.AcademicYear != null
                    && cs.Class.AcademicYear.StartDate != null) // Đảm bảo StartDate không null
                .OrderByDescending(cs => cs.Class.AcademicYear.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ClassStudent>> FindAllStudentByIdIsActive(int studentId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.User)
                .Include(cs => cs.Class)
                .Where(cs => cs.UserId == studentId
                    && cs.IsActive == true
                    && cs.IsDelete == false
                    && cs.User.IsDelete == false
            )
                .ToListAsync(); // Thay đổi FirstOrDefaultAsync thành ToListAsync
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
        public async Task<ClassStudent?> FindByUserId(int userId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .FirstOrDefaultAsync(cs => cs.UserId == userId && cs.IsDelete == false);
        }
        public async Task<List<ClassStudent>> FindAllClassStudentByUserId(int userId)
        {
            return await _context.ClassStudents
            .Include(cs => cs.Class)
            .Include(cs => cs.Class != null ? cs.Class.AcademicYear : null)
            .Include(cs => cs.Class != null ? cs.Class.Department : null)
            .Include(cs => cs.Class != null ? cs.Class.ClassSubjects : null)
            .Where(cs => cs.UserId == userId && cs.IsDelete == false)
            .ToListAsync(); ;
        }
        public async Task<ClassStudent?> FindByUserIdAndSchoolYearAndClassId(int userId, int schoolYearId, int classId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .ThenInclude(c => c.AcademicYear)
                .FirstOrDefaultAsync(cs => cs.UserId == userId
                    && cs.Class != null
                    && cs.Class.AcademicYear != null
                    && cs.Class.AcademicYear.Id == schoolYearId
                    && cs.ClassId == classId
                    && cs.IsDelete == false);
        }
        public async Task UpdateAsync(ClassStudent classStudent)
        {
            var existingClassStudent = await _context.ClassStudents.FindAsync(classStudent.Id);
            if (existingClassStudent != null)
            {
                Console.WriteLine("existingClassStudent: " + existingClassStudent.Id);
                existingClassStudent.UserId = classStudent.UserId;
                existingClassStudent.ClassId = classStudent.ClassId;
                existingClassStudent.IsActive = classStudent.IsActive;
                existingClassStudent.IsDelete = classStudent.IsDelete;
                existingClassStudent.UserUpdate = classStudent.UserUpdate;
                existingClassStudent.IsClassTransitionStatus = classStudent.IsClassTransitionStatus;

                _context.ClassStudents.Update(existingClassStudent);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<ClassStudent> FindByUserIdAndSchoolYear(int userId, int schoolYear)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class) // Bao gồm thông tin lớp để truy cập AcademicYearId
                .FirstOrDefaultAsync(cs => cs.UserId == userId && cs.Class.AcademicYearId == schoolYear && cs.IsDelete == false && cs.IsActive == true);
        }
        public async Task<ClassStudent> GetClassStudentChangeInfo(int userId, int classId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.User)
                .Include(cs => cs.Class)
                .FirstOrDefaultAsync(cs => cs.UserId == userId
                    && cs.ClassId == classId
                    && cs.IsDelete == false && cs.IsActive == true);
        }
        public async Task<IEnumerable<ClassStudent>> FindStudentByStudentDepartment(int studentId, int departmentId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                .Where(cs => cs.UserId == studentId && cs.Class.DepartmentId == departmentId && cs.IsActive == true
                 && cs.IsDelete == false && cs.Class.IsDelete == false && cs.User.IsDelete == false && cs.Class.Department.IsDelete == false)
                .ToListAsync();
        }

        public async Task<int> CountByClassId(int classId)
        {
            return await _context.ClassStudents
                .Where(cs => cs.ClassId == classId && cs.IsDelete == false && cs.IsActive == true)
                .CountAsync();
        }
    }
}
