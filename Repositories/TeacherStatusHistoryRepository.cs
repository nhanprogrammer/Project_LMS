using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class TeacherStatusHistoryRepository : ITeacherStatusHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherStatusHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherStatusHistory> AddAsync(TeacherStatusHistory teacher, string statusName)
        {
            var active = await _context.TeacherStatusHistories.Where(t => t.IsActive == true && t.UserId == teacher.UserId && t.IsDelete == false).FirstOrDefaultAsync();
            if (active != null) {
                active.IsActive = false;
                _context.TeacherStatusHistories.Update(active);
            }
            await _context.SaveChangesAsync();
            var teacherStatus = await _context.TeacherStatuses.FirstOrDefaultAsync(t => t.StatusName.ToLower().Contains(statusName.ToLower()));
            teacher.TeacherStatusId = teacherStatus.Id;
            await _context.TeacherStatusHistories.AddAsync(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task Delete(TeacherStatusHistory teacher)
        {

            teacher.IsDelete = true;

            _context.TeacherStatusHistories.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<TeacherStatusHistory> GetByTeacher(int teacherId)
        {
            return await _context.TeacherStatusHistories.FirstOrDefaultAsync(t => t.IsActive == true && t.UserId == teacherId && t.IsDelete == false);
        }

        public Task<TeacherStatusHistory> Update(TeacherStatusHistory teacher)
        {
            throw new NotImplementedException();
        }
    }
}
