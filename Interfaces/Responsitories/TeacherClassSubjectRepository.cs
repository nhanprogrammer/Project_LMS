using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public class TeacherClassSubjectRepository: ITeacherClassSubjectService
    {
        private readonly ApplicationDbContext _context;

        public TeacherClassSubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeacherClassSubject>> GetAll()
        {
            return await _context.TeacherClassSubjects.ToListAsync();
        }

        public async Task<TeacherClassSubject?> GetById(int id)
        {
            return await _context.TeacherClassSubjects.FindAsync(id);
        }

        public async Task<TeacherClassSubject> Add(TeacherClassSubject teacherClassSubject)
        {
            _context.TeacherClassSubjects.Add(teacherClassSubject);
            await _context.SaveChangesAsync();
            return teacherClassSubject;
        }

        public async Task<TeacherClassSubject?> Update(int id, TeacherClassSubject teacherClassSubject)
        {
            var existing = await _context.TeacherClassSubjects.FindAsync(id);
            if (existing == null) return null;

            existing.UserId = teacherClassSubject.UserId;
            existing.SubjectsId = teacherClassSubject.SubjectsId;
            existing.IsPrimary = teacherClassSubject.IsPrimary;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var teacherClassSubject = await _context.TeacherClassSubjects.FindAsync(id);
            if (teacherClassSubject == null) return false;

            _context.TeacherClassSubjects.Remove(teacherClassSubject);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
