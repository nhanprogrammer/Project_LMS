using System.Net.WebSockets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {

            user.StudentStatus = await _context.StudentStatuses
                .FirstOrDefaultAsync(s => s.Id == user.StudentStatusId)
                ?? throw new ArgumentException($"StudentStatus with Id {user.StudentStatusId} not found.");


            user.Role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.Equals("Student"))
                ?? throw new ArgumentException("Role 'Student' not found.");


            await _context.Users.AddAsync(user);


            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<int> CountStudentOfRewardByIds(bool isReward, List<int> ids, string searchItem)
        {
            var query = _context.Users
          .Include(u => u.Rewards).AsSplitQuery()
          .Where(u => isReward ? (u.Rewards.Count > 0) : (u.Disciplines.Count > 0) && u.IsDelete == false && u.Role.Name.Equals("Student"));
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.Trim().ToLower();
                query = query.Where(st =>
                (st.UserCode != null && st.UserCode.ToLower().Contains(searchItem)) ||
                (st.FullName != null && st.FullName.ToLower().Contains(searchItem))
                );
            }
            return await query.CountAsync();
        }

        public async Task<User> FindStudentByEmailOrderUserCode(string email, string userCode)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserCode != userCode && u.IsDelete ==false &&  u.Role.Name.ToLower().Contains("student"));
        }

        public async Task<User> FindStudentById(int studentId)
        {
            return await _context.Users
                    .Include(u => u.Assignments).ThenInclude(asm => asm.TestExam).ThenInclude(te => te.Semesters)
                    .Include(u => u.Assignments).ThenInclude(asm => asm.TestExam).ThenInclude(te => te.TestExamType)
                    .Include(u => u.Assignments).ThenInclude(asm => asm.TestExam).ThenInclude(te => te.Subject)
                    .Include(u => u.Rewards)
                    .Include(u => u.Disciplines)
                    .Include(u => u.Role)
                    .Include(u => u.StudentStatus)
                    .AsSplitQuery()
                    .Where(u => u.Id == studentId && u.IsDelete ==false  && u.Role.Name.ToLower().Contains("student"))
                    .FirstOrDefaultAsync();
        }

        public async Task<User> FindStudentByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsDelete == false && u.Role.Name.ToLower().Contains("student"));
        }

        public async Task<User> FindStudentByUserCode(string userCode)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserCode == userCode && u.IsDelete == false && u.Role.Name.ToLower().Contains("student"));
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<User>> GetAllOfRewardByIds(bool isReward, List<int> ids, PaginationRequest request, string columnm, bool orderBy, string searchItem)
        {
            var query = _context.Users
                .Include(u => u.Rewards)
                .Include(u=>u.Disciplines)
                .AsSplitQuery()
                .Where(u => isReward ? (u.Rewards.Count > 0) : (u.Disciplines.Count > 0) && u.IsDelete == false && u.Role.Name.Equals("Student"));
            if (!string.IsNullOrWhiteSpace(searchItem))
            {
                searchItem = searchItem.Trim().ToLower();
                query = query.Where(st =>
                (st.UserCode != null && st.UserCode.ToLower().Contains(searchItem)) ||
                (st.FullName != null && st.FullName.ToLower().Contains(searchItem))
                );
            }
            switch (columnm.ToLower())
            {
                case "usercode":
                    query = orderBy ? query.OrderBy(st => st.UserCode ?? string.Empty)
                                    : query.OrderByDescending(st => st.UserCode ?? string.Empty);
                    break;
                case "fullname":
                    query = orderBy ? query.OrderBy(st => st.FullName ?? string.Empty)
                                    : query.OrderByDescending(st => st.FullName ?? string.Empty);
                    break;
                case "birthdate":
                    query = orderBy ? query.OrderBy(st => st.BirthDate)
                                    : query.OrderByDescending(st => st.BirthDate);
                    break;
                case "gender":
                    query = orderBy ? query.OrderBy(st => st.Gender)
                                    : query.OrderByDescending(st => st.Gender);
                    break;
                case "reward":
                    query = orderBy ? query.OrderBy(st => st.Rewards.Count)
                                    : query.OrderByDescending(st => st.Rewards.Count);
                    break;
                default:
                    query = orderBy ? query.OrderBy(st => st.UserCode ?? string.Empty)
                                  : query.OrderByDescending(st => st.UserCode ?? string.Empty);
                    break;

            }
            return await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.StudentStatus = await _context.StudentStatuses
                    .FirstOrDefaultAsync(s => s.Id == user.StudentStatusId)
                    ?? throw new ArgumentException($"StudentStatus with Id {user.StudentStatusId} not found.");
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> FindStudentByUsername(string username)
        {
            return await _context.Users.FirstAsync(u => u.Username == username && u.IsDelete == false && u.Role.Name.ToLower().Contains("student"));
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddAllAsync(List<User> users)
        {
           _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    }
}