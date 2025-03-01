using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;

namespace Project_LMS.Services
{
    public class QuestionsAnswerTopicViewService : IQuestionsAnswerTopicViewService
    {
        private readonly IQuestionsAnswerTopicViewRepository _questionsAnswerTopicViewRepository;

        public QuestionsAnswerTopicViewService(IQuestionsAnswerTopicViewRepository questionsAnswerTopicViewRepository)
        {
            _questionsAnswerTopicViewRepository = questionsAnswerTopicViewRepository;
        }

        public async Task<IEnumerable<QuestionsAnswerTopicViewResponse>> GetAllAsync()
        {
            var questionsAnswerTopicViews = await _questionsAnswerTopicViewRepository.GetAllAsync();
            return questionsAnswerTopicViews.Select(q => new QuestionsAnswerTopicViewResponse
            {
                Id = q.Id,
                QuestionsAnswerId = q.QuestionsAnswerId,
                UserId = q.UserId,
                TopicId = q.TopicId
            });
        }

        public async Task<QuestionsAnswerTopicViewResponse> GetByIdAsync(int id)
        {
            var questionsAnswerTopicView = await _questionsAnswerTopicViewRepository.GetByIdAsync(id);
            if (questionsAnswerTopicView == null)
            {
                return null;
            }
            return new QuestionsAnswerTopicViewResponse
            {
                Id = questionsAnswerTopicView.Id,
                QuestionsAnswerId = questionsAnswerTopicView.QuestionsAnswerId,
                UserId = questionsAnswerTopicView.UserId,
                TopicId = questionsAnswerTopicView.TopicId
            };
        }

        public async Task<QuestionsAnswerTopicViewResponse> CreateAsync(QuestionsAnswerTopicViewRequest request)
        {

            if (request.QuestionsAnswerId == null || request.UserId == null || request.TopicId == null)
            {
                throw new ArgumentNullException("QuestionsAnswerId, UserId, và TopicId không được phép là null.");
            }

            var questionsAnswerTopicView = new QuestionsAnswerTopicView
            {
                QuestionsAnswerId = request.QuestionsAnswerId.Value,
                UserId = request.UserId.Value,
                TopicId = request.TopicId.Value
            };

            await _questionsAnswerTopicViewRepository.AddAsync(questionsAnswerTopicView);

            return new QuestionsAnswerTopicViewResponse
            {
                Id = questionsAnswerTopicView.Id,
                QuestionsAnswerId = questionsAnswerTopicView.QuestionsAnswerId,
                UserId = questionsAnswerTopicView.UserId,
                TopicId = questionsAnswerTopicView.TopicId
            };
        }

        public async Task<QuestionsAnswerTopicViewResponse> UpdateAsync(int id, QuestionsAnswerTopicViewRequest request)
        {

            var questionsAnswerTopicView = await _questionsAnswerTopicViewRepository.GetByIdAsync(id);
            if (questionsAnswerTopicView == null)
            {
                throw new NotFoundException("Bản ghi không tồn tại.");
            }


            if (request.QuestionsAnswerId == null || request.UserId == null || request.TopicId == null)
            {
                throw new ArgumentNullException("QuestionsAnswerId, UserId, và TopicId không được phép là null.");
            }


            questionsAnswerTopicView.QuestionsAnswerId = request.QuestionsAnswerId.Value;
            questionsAnswerTopicView.UserId = request.UserId.Value;
            questionsAnswerTopicView.TopicId = request.TopicId.Value;

            await _questionsAnswerTopicViewRepository.UpdateAsync(questionsAnswerTopicView);

            return new QuestionsAnswerTopicViewResponse
            {
                Id = questionsAnswerTopicView.Id,
                QuestionsAnswerId = questionsAnswerTopicView.QuestionsAnswerId,
                UserId = questionsAnswerTopicView.UserId,
                TopicId = questionsAnswerTopicView.TopicId
            };
        }

        public async Task<QuestionsAnswerTopicViewResponse> DeleteAsync(int id)
        {
            var questionsAnswerTopicView = await _questionsAnswerTopicViewRepository.GetByIdAsync(id);
            if (questionsAnswerTopicView == null)
            {
                return null;
            }
            await _questionsAnswerTopicViewRepository.DeleteAsync(id);
            return new QuestionsAnswerTopicViewResponse
            {
                Id = questionsAnswerTopicView.Id,
                QuestionsAnswerId = questionsAnswerTopicView.QuestionsAnswerId,
                UserId = questionsAnswerTopicView.UserId,
                TopicId = questionsAnswerTopicView.TopicId
            };
        }
    }
}