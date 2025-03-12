// using Microsoft.EntityFrameworkCore;
// using Project_LMS.Data;
// using Project_LMS.Interfaces.Repositories;
// using Project_LMS.Models;

// namespace Project_LMS.Repositories
// {
//     public class SubjectRepository : ISubjectRepository
//     {
//         private readonly ApplicationDbContext _context;

//         public SubjectRepository(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         public async Task<IEnumerable<Subject>> GetAllSubjects(int pageNumber, int pageSize)
//         {
//             return await _context.Subjects
//                 .Where(s => !s.IsDelete.HasValue || !s.IsDelete.Value)
//                 .Include(s => s.SubjectType)
//                 .Include(s => s.SubjectGroup)
//                 .Skip((pageNumber - 1) * pageSize)
//                 .Take(pageSize)
//                 .ToListAsync();
//         }

//         public async Task<Subject?> GetSubjectById(int id)
//         {
//             return await _context.Subjects
//                 .Include(s => s.SubjectType)
//                 .Include(s => s.SubjectGroup)
//                 .FirstOrDefaultAsync(s => s.Id == id && (!s.IsDelete.HasValue || !s.IsDelete.Value));
//         }

//         public async Task<Subject> AddSubject(Subject subject)
//         {
//             _context.Subjects.Add(subject);
//             await _context.SaveChangesAsync();
//             return subject;
//         }

//         public async Task<Subject?> UpdateSubject(int id, Subject subject)
//         {
//             var existingSubject = await _context.Subjects.FindAsync(id);
//             if (existingSubject == null || existingSubject.IsDelete == true)
//                 return null;

//             existingSubject.SubjectTypeId = subject.SubjectTypeId;
//             // existingSubject.TeachingAssignmentId = subject.TeachingAssignmentId;
//             // existingSubject.SubjectGroupId = subject.SubjectGroupId;
//             existingSubject.IsStatus = subject.IsStatus;
//             existingSubject.SubjectCode = subject.SubjectCode;
//             existingSubject.SubjectName = subject.SubjectName;
//             existingSubject.Description = subject.Description;
//             existingSubject.Semester1PeriodCount = subject.Semester1PeriodCount;
//             existingSubject.Semester2PeriodCount = subject.Semester2PeriodCount;
//             existingSubject.UpdateAt = DateTime.UtcNow;
//             existingSubject.UserUpdate = subject.UserUpdate;

//             await _context.SaveChangesAsync();
//             return existingSubject;
//         }

//         public async Task<bool> DeleteSubject(int id)
//         {
//             var subject = await _context.Subjects.FindAsync(id);
//             if (subject == null || subject.IsDelete == true)
//                 return false;

//             subject.IsDelete = true;
//             subject.UpdateAt = DateTime.UtcNow;
            
//             await _context.SaveChangesAsync();
//             return true;
//         }
//     }
// }