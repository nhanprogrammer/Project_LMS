using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Models;
using Project_LMS.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Repositories
{
    public class QuestionsAnswerTopicViewRepository : IQuestionsAnswerTopicViewRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionsAnswerTopicViewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestionAnswerTopicView>> GetAllAsync()
        {
            return await _context.QuestionAnswerTopicViews.ToListAsync();
        }

        public async Task<QuestionAnswerTopicView?> GetByIdAsync(int id)
        {
            return await _context.QuestionAnswerTopicViews.FindAsync(id);
        }

        public async Task AddAsync(QuestionAnswerTopicView questionsAnswerTopicView)
        {
            await _context.QuestionAnswerTopicViews.AddAsync(questionsAnswerTopicView);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionAnswerTopicView questionsAnswerTopicView)
        {
            _context.QuestionAnswerTopicViews.Update(questionsAnswerTopicView);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var questionsAnswerTopicView = await _context.QuestionAnswerTopicViews.FindAsync(id);
            if (questionsAnswerTopicView != null)
            {
                _context.QuestionAnswerTopicViews.Remove(questionsAnswerTopicView);
                await _context.SaveChangesAsync();
            }
        }
    }
}