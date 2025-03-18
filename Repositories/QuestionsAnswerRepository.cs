using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
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


        public async Task<PaginatedResponse<QuestionAnswer>> GetAllAsync(PaginationRequest request)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                var allItems = await _context.QuestionAnswers.Where(c => c.IsDelete == false).ToListAsync();
                return new PaginatedResponse<QuestionAnswer>
                {
                    Items = allItems,
                    PageNumber = 1,
                    PageSize = allItems.Count,
                    TotalItems = allItems.Count,
                    TotalPages = 1,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
            }

            var query = _context.QuestionAnswers.Where(c => c.IsDelete == false);
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponse<QuestionAnswer>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber < totalPages
            };
        }

        public async Task<QuestionAnswer?> GetByIdAsync(int id)
        {
            return await _context.QuestionAnswers.FindAsync(id);
        }

        public async Task<QuestionAnswer?> AddAsync(QuestionAnswer questionsAnswer, int topicId, int userId)
        {
            _context.QuestionAnswers.Add(questionsAnswer);
            await _context.SaveChangesAsync();

            var addedQuestionAnswer = new QuestionAnswerTopicView()
            {
                QuestionsAnswerId = questionsAnswer.Id,
                TopicId = topicId,
                CreateAt = TimeHelper.NowUsingTimeZone,
                UserId = userId,
                IsDelete = false
            };
            _context.QuestionAnswerTopicViews.Add(addedQuestionAnswer);
            await _context.SaveChangesAsync();

            return questionsAnswer;
        }

        public async Task<QuestionAnswer?> UpdateAsync(QuestionAnswer updatedComment, int? newTopicId = null)
        {
            // 1. Tìm bản ghi bình luận hiện tại từ bảng QuestionAnswers
            var existingComment = await _context.QuestionAnswers
                .FirstOrDefaultAsync(q => q.Id == updatedComment.Id && q.IsDelete == false);

            if (existingComment == null)
            {
                return null;
            }

            // 2. Cập nhật các trường cần thiết của bình luận
            existingComment.Message = updatedComment.Message;
            existingComment.FileName = updatedComment.FileName;
            existingComment.UpdateAt = TimeHelper.NowUsingTimeZone;
            existingComment.UserUpdate = updatedComment.UserUpdate;

            _context.QuestionAnswers.Update(existingComment);
            await _context.SaveChangesAsync();

            // 3. Nếu có newTopicId, kiểm tra và cập nhật bảng liên kết (nếu cần thay đổi topic)
            if (newTopicId.HasValue)
            {
                var mapping = await _context.QuestionAnswerTopicViews
                    .FirstOrDefaultAsync(m => m.QuestionsAnswerId == updatedComment.Id && m.IsDelete == false);

                if (mapping != null && mapping.TopicId != newTopicId.Value)
                {
                    mapping.TopicId = newTopicId.Value;
                    mapping.UpdateAt = TimeHelper.NowUsingTimeZone;
                    _context.QuestionAnswerTopicViews.Update(mapping);
                    await _context.SaveChangesAsync();
                }
            }

            return existingComment;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var questionsAnswer = await _context.Topics.FindAsync(id);
            if (questionsAnswer == null)
            {
                return false;
            }

            questionsAnswer.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuestionAnswer>> GetAllQuestionAnswerByTopicIdAsync(int topicId)
        {
            var questionAnswers = await (
                from qa in _context.QuestionAnswers
                join qatv in _context.QuestionAnswerTopicViews
                    on qa.Id equals qatv.QuestionsAnswerId
                where qatv.TopicId == topicId && qa.IsDelete == false
                select qa
            ).ToListAsync();

            return questionAnswers;
        }
    }
}