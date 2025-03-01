using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories
{
    public class QuestionsAnswerRepository : IQuestionsAnswerRepository
    {
        private readonly ApplicationDbContext _context;
        
        public QuestionsAnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<QuestionsAnswer>> GetAllAsync()
        {
            return await _context.QuestionsAnswers
                .Where(q => q.IsDelete == false || q.IsDelete == null)
                .Include(q => q.User)
                .Include(q => q.TeachingAssignment)
                .Include(q => q.QuestionsAnswerNavigation)
                .ToListAsync();
        }

        public async Task<QuestionsAnswer?> GetByIdAsync(int id)
        {
            return await _context.QuestionsAnswers
                .Include(q => q.User)
                .Include(q => q.TeachingAssignment)
                .Include(q => q.QuestionsAnswerNavigation)
                .FirstOrDefaultAsync(q => q.Id == id && (q.IsDelete == false || q.IsDelete == null));
        }

        public async Task AddAsync(QuestionsAnswer questionsAnswer)
        {
            questionsAnswer.CreateAt = DateTime.UtcNow;
            _context.QuestionsAnswers.Add(questionsAnswer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionsAnswer questionsAnswer)
        {
            questionsAnswer.UpdateAt = DateTime.UtcNow;
            _context.QuestionsAnswers.Update(questionsAnswer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var questionsAnswer = await _context.QuestionsAnswers.FindAsync(id);
            if (questionsAnswer != null)
            {
                questionsAnswer.IsDelete = true;
                questionsAnswer.UpdateAt = DateTime.UtcNow;
                _context.QuestionsAnswers.Update(questionsAnswer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
