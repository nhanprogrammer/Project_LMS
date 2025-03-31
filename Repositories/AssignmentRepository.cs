using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment> GetByIdAsync(int id)
    {
        return await _context.Assignments.FindAsync(id);
    }

    public async Task<IEnumerable<Assignment>> GetAllAsync()
    {
        return await _context.Assignments.ToListAsync();
    }

    public async Task AddAsync(Assignment entity)
    {
        await _context.Assignments.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment entity)
    {
        _context.Assignments.Update(entity);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment != null)
        {
            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Assignment>> GetAllByClassAndSubjectAndSemesterAndSearch(int classId, int subjectId, int semesterId, string searchItem)
    {
        var query = _context.Assignments
            .Include(asm => asm.TestExam.Class)
            .Include(asm => asm.TestExam.Subject)
            .Include(asm => asm.User)
            .Where(asm => asm.IsDelete == false && asm.TestExam.SemestersId == semesterId && asm.TestExam.ClassId == classId && asm.TestExam.SubjectId == subjectId && asm.TotalScore >= 0 && asm.TestExam.IsDelete == false && asm.User.IsDelete == false);
        if (!string.IsNullOrWhiteSpace(searchItem))
        {
            query.Where
                (asm =>
                asm.User.UserCode.ToLower().Contains(searchItem.ToLower()) ||
                asm.User.FullName.ToLower().Contains(searchItem.ToLower())
                );
        }
        return await query.ToListAsync();
    }

    public async Task<double> AvgScoreByStudentAndClassAndSubjectAndSearch(int studentId, int classId, int subjectId)
    {
        // Truy vấn chung cho cả hai học kỳ
        var querySemester1 = _context.Assignments
            .Include(asm => asm.TestExam)
                .ThenInclude(te => te.Class)
            .Include(asm => asm.TestExam)
                .ThenInclude(te => te.Subject)
            .Include(asm => asm.User)
            .Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 1")
                && asm.TestExam.IsDelete == false
                && asm.User.IsDelete == false
                && asm.TestExam.ClassId == classId
                && asm.UserId == studentId
                && asm.TestExam.SubjectId == subjectId
                && asm.TotalScore >= 0);

        var querySemester2 = _context.Assignments
            .Include(asm => asm.TestExam)
                .ThenInclude(te => te.Class)
            .Include(asm => asm.TestExam)
                .ThenInclude(te => te.Subject)
            .Include(asm => asm.User)
            .Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 2")
                && asm.IsDelete == false
                && asm.TestExam.IsDelete == false
                && asm.User.IsDelete == false
                && asm.TestExam.ClassId == classId
                && asm.UserId == studentId
                && asm.TestExam.SubjectId == subjectId
                && asm.TotalScore >= 0);

        // Tải dữ liệu một lần để tối ưu hiệu suất
        var resultSemester1 = await querySemester1
            .Select(asm => new
            {
                Coefficient = asm.TestExam.TestExamType.Coefficient ?? 0,
                Score = asm.TotalScore.Value
            })
            .ToListAsync();

        var resultSemester2 = await querySemester2
            .Select(asm => new
            {
                Coefficient = asm.TestExam.TestExamType.Coefficient ?? 0,
                Score = asm.TotalScore.Value
            })
            .ToListAsync();

        // Tính tổng hệ số và tổng điểm sau khi tải dữ liệu
        double totalCoefficientSemester1 = resultSemester1.Sum(x => x.Coefficient);
        double totalScoreSemester1 = resultSemester1.Sum(x => x.Score * x.Coefficient);

        double totalCoefficientSemester2 = resultSemester2.Sum(x => x.Coefficient);
        double totalScoreSemester2 = resultSemester2.Sum(x => x.Score * x.Coefficient);

        // Tránh chia cho 0
        double avgSemester1 = totalCoefficientSemester1 > 0 ? totalScoreSemester1 / totalCoefficientSemester1 : 0;
        double avgSemester2 = totalCoefficientSemester2 > 0 ? totalScoreSemester2 / totalCoefficientSemester2 : 0;

        // Công thức trung bình có trọng số (học kỳ 2 nhân đôi)
        return await querySemester2.CountAsync() >0? (avgSemester1 + (avgSemester2 * 2)) / 3 : avgSemester1;
    }

}