
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name.Equals("Teacher"));
            user.Role = role;
            user.CreateAt = DateTime.Now;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<int> CountByClasses(List<int> ids, string searchItem)
        {
            var query = _context.Users.Include(u => u.SubjectGroup)
                             .Include(u => u.TeacherStatus)
                             .Include(u => u.Role)
                             .Where(u => ids.Contains(u.Id) && u.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.ToLower(); // Không phân biệt hoa thường
                query = query.Where(cs =>
                    (cs.UserCode.ToLower().Contains(searchItem)) ||
                    (cs.FullName.ToLower().Contains(searchItem)) ||
                    (cs.SubjectGroup.Name.ToLower().Contains(searchItem)) ||
                    (cs.Role.Name.ToLower().Contains(searchItem)) ||
                    (cs.TeacherStatus.StatusName.ToLower().Contains(searchItem))
                );
            }
            return await query.CountAsync();
        }

        public async Task DeleteAsync(User user)
        {
            user.IsDelete = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindTeacherByEmailOrderUserCode(string email, string userCode)
        {
            return await _context.Users.Where(u => (!(u.UserCode.Equals(userCode)) || userCode == null) && u.Email.Equals(email)).FirstOrDefaultAsync();
        }

        public async Task<User> FindTeacherByUserCode(string userCode)
        {
            return await _context.Users
                .Include(u => u.SubjectGroup)
                .Include(u => u.TeacherStatus)
                .Include(u => u.TeacherClassSubjects).ThenInclude(tcs => tcs.Subjects)
                .Include(u => u.Departments)
                .Where(u => u.UserCode == userCode && u.IsDelete == false && u.Role.Name.ToLower().Contains("teacher")).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllByIds(List<int> ids, PaginationRequest request, bool orderBy, string column, string searchItem)
        {
            var query = _context.Users.Include(u => u.SubjectGroup)
                 .Include(u => u.TeacherStatus)
                 .Include(u => u.Role)
                 .Where(u => ids.Contains(u.Id) && u.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.ToLower(); // Không phân biệt hoa thường
                query = query.Where(cs =>
                    (cs.UserCode.ToLower().Contains(searchItem)) ||
                    (cs.FullName.ToLower().Contains(searchItem)) ||
                    (cs.SubjectGroup.Name.ToLower().Contains(searchItem)) ||
                    (cs.Role.Name.ToLower().Contains(searchItem)) ||
                    (cs.TeacherStatus.StatusName.ToLower().Contains(searchItem))
                );
            }
            switch (column?.ToLower())
            {
                case "usercode":
                    query = orderBy
                        ? query.OrderBy(u => u.UserCode)
                        : query.OrderByDescending(u => u.UserCode);
                    break;
                case "fullname":
                    query = orderBy
                        ? query.OrderBy(u => u.FullName)
                        : query.OrderByDescending(u => u.FullName);
                    break;
                case "birthdate":
                    query = orderBy
                        ? query.OrderBy(u => u.BirthDate)
                        : query.OrderByDescending(u => u.BirthDate);
                    break;
                case "gender":
                    query = orderBy
                        ? query.OrderBy(u => u.Gender)
                        : query.OrderByDescending(u => u.Gender);
                    break;
                case "subjectgroup":
                    query = orderBy
                        ? query.OrderBy(u => u.SubjectGroup.Name)
                        : query.OrderByDescending(u => u.SubjectGroup.Name);
                    break;
                case "rolename":
                    query = orderBy
                        ? query.OrderBy(u => u.Role.Name)
                        : query.OrderByDescending(u => u.Role.Name);
                    break;
                case "teacherstatus":
                    query = orderBy
                        ? query.OrderBy(u => u.TeacherStatus.StatusName)
                        : query.OrderByDescending(u => u.TeacherStatus.StatusName);
                    break;
                default:
                    query = orderBy
                        ? query.OrderBy(u => u.UserCode)
                        : query.OrderByDescending(u => u.UserCode);
                    break;
            }
            return await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllByIds(List<int> ids, bool orderBy, string column, string searchItem)
        {
            var query = _context.Users.Include(u => u.SubjectGroup)
           .Include(u => u.TeacherStatus)
           .Include(u => u.Role)
           .Where(u => ids.Contains(u.Id) && u.IsDelete == false);
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.ToLower(); // Không phân biệt hoa thường
                query = query.Where(cs =>
                (cs.UserCode.ToLower().Contains(searchItem)) ||
                (cs.FullName.ToLower().Contains(searchItem)) ||
                    (cs.SubjectGroup.Name.ToLower().Contains(searchItem)) ||
                    (cs.Role.Name.ToLower().Contains(searchItem)) ||
                    (cs.TeacherStatus.StatusName.ToLower().Contains(searchItem))
                );
            }
            switch (column?.ToLower())
            {
                case "usercode":
                    query = orderBy
                        ? query.OrderBy(u => u.UserCode)
                        : query.OrderByDescending(u => u.UserCode);
                    break;
                case "fullname":
                    query = orderBy
                        ? query.OrderBy(u => u.FullName)
                        : query.OrderByDescending(u => u.FullName);
                    break;
                case "birthdate":
                    query = orderBy
                        ? query.OrderBy(u => u.BirthDate)
                        : query.OrderByDescending(u => u.BirthDate);
                    break;
                case "gender":
                    query = orderBy
                        ? query.OrderBy(u => u.Gender)
                        : query.OrderByDescending(u => u.Gender);
                    break;
                case "subjectgroup":
                    query = orderBy
                        ? query.OrderBy(u => u.SubjectGroup.Name)
                        : query.OrderByDescending(u => u.SubjectGroup.Name);
                    break;
                case "rolename":
                    query = orderBy
                        ? query.OrderBy(u => u.Role.Name)
                        : query.OrderByDescending(u => u.Role.Name);
                    break;
                case "teacherstatus":
                    query = orderBy
                        ? query.OrderBy(u => u.TeacherStatus.StatusName)
                        : query.OrderByDescending(u => u.TeacherStatus.StatusName);
                    break;
                default:
                    query = orderBy
                        ? query.OrderBy(u => u.UserCode)
                        : query.OrderByDescending(u => u.UserCode);
                    break;
            }
            return await query.ToListAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdateAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<List<UserResponseTeachingAssignment>> GetTeachersAsync()
        {
            var teachers = await _context.Users
                .Where(u => u.RoleId == 2 && u.TeacherStatusId == 1 && (u.IsDelete == false || u.IsDelete == null))
                .Select(u => new UserResponseTeachingAssignment
                {
                    Id = u.Id,
                    FullName = u.FullName
                })
                .ToListAsync(); // Lấy dữ liệu từ DB trước

            // Sắp xếp trong bộ nhớ
            return teachers.OrderBy(u => u.FullName != null ? u.FullName.Substring(u.FullName.LastIndexOf(' ') + 1) : string.Empty).ToList();
        }

        public async Task<List<UserResponseTeachingAssignment>> GetTeacherBySubjectIdAsync(int subjectId)
        {
            // Sửa lại cách truy vấn và quan hệ
            var teachers = await _context.Users
                .Where(u => u.RoleId == 2 &&
                           u.TeacherStatusId == 1 &&
                           (u.IsDelete == false || u.IsDelete == null))
                .Where(u => u.SubjectGroup != null &&
                           u.SubjectGroup.SubjectGroupSubjects
                             .Any(sgs => sgs.Subject != null && sgs.Subject.Id == subjectId))
                .Select(u => new UserResponseTeachingAssignment
                {
                    Id = u.Id,
                    FullName = u.FullName ?? string.Empty // Xử lý null
                })
                .ToListAsync();

            // Sắp xếp trong bộ nhớ với xử lý null an toàn
            return teachers
                .OrderBy(u =>
                {
                    if (string.IsNullOrEmpty(u.FullName))
                        return string.Empty;

                    int lastSpaceIndex = u.FullName.LastIndexOf(' ');
                    return lastSpaceIndex >= 0 ?
                        u.FullName.Substring(lastSpaceIndex + 1) :
                        u.FullName;
                })
                .ToList();
        }
    }
}