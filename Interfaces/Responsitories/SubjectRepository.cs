using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Interfaces.Responsitories
{
    public class SubjectRepository: ISubjectService
    {
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjects()
        {
            return await _context.Subjects
                // .Include(s => s.TypeSubject) Sửa code chỗ này
                .Include(s => s.SubjectGroup)
                .Where(s => s.IsDelete == false || s.IsDelete == null) // Lọc những môn chưa bị xóa
                .ToListAsync();
        }

        public async Task<Subject?> GetSubjectById(int id)
        {
            return await _context.Subjects
                // .Include(s => s.TypeSubject) Sửa code chỗ này
                .Include(s => s.SubjectGroup)
                .FirstOrDefaultAsync(s => s.Id == id && (s.IsDelete == false || s.IsDelete == null)); // Lọc những môn chưa bị xóa
        }

        public async Task<Subject> AddSubject(Subject subject)
        {
            subject.CreateAt = DateTime.UtcNow;
            subject.IsDelete = false;
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<Subject?> UpdateSubject(int id, Subject subject)
        {
            var existingSubject = await _context.Subjects.FindAsync(id);
            if (existingSubject == null) return null;

            // existingSubject.TypeSubjectId = subject.TypeSubjectId; Sửa code chỗ này
            existingSubject.SubjectGroupId = subject.SubjectGroupId;
            existingSubject.IsStatus = subject.IsStatus;
            existingSubject.Description = subject.Description;
            existingSubject.UserUpdate = subject.UserUpdate;
            existingSubject.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingSubject;
        }

        public async Task<bool> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return false;

            subject.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
