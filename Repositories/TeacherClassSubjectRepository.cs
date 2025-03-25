using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class TeacherClassSubjectRepository : ITeacherClassSubjectRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherClassSubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<TeacherClassSubject> FindSubjectByTeacherIsPrimary(int? teacherId)
        {
            return await _context.TeacherClassSubjects.FirstOrDefaultAsync(t => t.UserId == teacherId && t.IsPrimary == true && t.User.IsDelete == false && t.IsDelete == false);
        }
        public async Task<List<TeacherClassSubject>> GetAllByTeacher(int? teacherId)
        {
            return await _context.TeacherClassSubjects.Where(t => t.UserId == teacherId && t.User.IsDelete == false && t.IsDelete == false).ToListAsync();
        }

        public async Task<TeacherClassSubject> AddAsync(TeacherClassSubject teacher)
        {
            await _context.TeacherClassSubjects.AddAsync(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task DeleteAsync(TeacherClassSubject teacherClassSubject)
        {
            teacherClassSubject.IsDelete = true;
            _context.TeacherClassSubjects.Update (teacherClassSubject);
            await _context.SaveChangesAsync();
        }
    }
}
