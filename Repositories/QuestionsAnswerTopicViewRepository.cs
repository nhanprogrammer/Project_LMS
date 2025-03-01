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

        public async Task<IEnumerable<QuestionsAnswerTopicView>> GetAllAsync()
        {
            return await _context.QuestionsAnswerTopicViews.ToListAsync();
        }

        public async Task<QuestionsAnswerTopicView?> GetByIdAsync(int id)
        {
            return await _context.QuestionsAnswerTopicViews.FindAsync(id);
        }

        public async Task AddAsync(QuestionsAnswerTopicView questionsAnswerTopicView)
        {
            await _context.QuestionsAnswerTopicViews.AddAsync(questionsAnswerTopicView);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuestionsAnswerTopicView questionsAnswerTopicView)
        {
            _context.QuestionsAnswerTopicViews.Update(questionsAnswerTopicView);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var questionsAnswerTopicView = await _context.QuestionsAnswerTopicViews.FindAsync(id);
            if (questionsAnswerTopicView != null)
            {
                _context.QuestionsAnswerTopicViews.Remove(questionsAnswerTopicView);
                await _context.SaveChangesAsync();
            }
        }
    }
}