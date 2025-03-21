using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class TestExamRepository : ITestExamRepository
{
    private readonly ApplicationDbContext _context;

    public TestExamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TestExam> GetByIdAsync(int id)
    {
        return await _context.TestExams
            .Where(t => t.IsDelete == false)
            .Include(t => t.Department)
            .Include(t => t.Examiners).ThenInclude(e => e.User)
            .Include(t => t.Semesters)
            .Include(t => t.ExamScheduleStatus)
            .Include(t => t.Subject)
            .Include(t => t.TestExamType)
            .Include(t => t.Class)
            .FirstOrDefaultAsync(sg => sg.Id == id) ?? throw new NotFoundException("TestExam not found");
    }


    public async Task<IEnumerable<TestExam>> GetAllAsync()
    {
      return  await _context.TestExams
            .Where(t => t.IsDelete == false)
            .Include(t => t.Department)
            .Include(t=> t.Examiners).ThenInclude(t=> t.User)
            .Include(t=>t.Semesters)
            .Include(t => t.ExamScheduleStatus)
            .Include(t=> t.Subject)
            .Include(t=>t.TestExamType)
            .ToListAsync();
    }

    public Task AddAsync(TestExam entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TestExam entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}