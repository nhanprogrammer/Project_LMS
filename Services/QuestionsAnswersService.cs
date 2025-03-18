using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Hubs;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class QuestionsAnswersService : IQuestionsAnswersService
    {
        private readonly IQuestionsAnswerRepository _questionsAnswerRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly IHubContext<RealtimeHub> _hubContext;
        private readonly ITopicRepository _topicRepository;
        private readonly IQuestionsAnswerTopicViewRepository _questionAnswerTopicViewRepository;

        public QuestionsAnswersService(IQuestionsAnswerRepository questionsAnswerRepository,
            ICloudinaryService cloudinaryService, IMapper mapper,
            ITopicRepository topicRepository, IQuestionsAnswerTopicViewRepository questionAnswerTopicViewRepository,
            IHubContext<RealtimeHub> hubContext)
        {
            _questionsAnswerRepository = questionsAnswerRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _topicRepository = topicRepository;
            _questionAnswerTopicViewRepository = questionAnswerTopicViewRepository;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<PaginatedResponse<QuestionsAnswerResponse>>> GetAllAsync(
            PaginationRequest request)
        {
            var response = await _questionsAnswerRepository.GetAllAsync(request);
            var data = _mapper.Map<PaginatedResponse<QuestionsAnswerResponse>>(response);
            return new ApiResponse<PaginatedResponse<QuestionsAnswerResponse>>(0, "Lấy danh sách thông tin thành công!",
                data);
        }

        public async Task<ApiResponse<QuestionsAnswerResponse?>> GetByIdAsync(int id)
        {
            var response = await _questionsAnswerRepository.GetByIdAsync(id);
            if (response == null)
            {
                return new ApiResponse<QuestionsAnswerResponse?>(1, "Không tìm thấy thông tin!", null);
            }

            var data = _mapper.Map<QuestionsAnswerResponse>(response);
            return new ApiResponse<QuestionsAnswerResponse?>(0, "Lấy thông tin thành công!", data);
        }

        public async Task<ApiResponse<QuestionsAnswerResponse?>> AddAsync(CreateQuestionsAnswerRequest request)
        {
            try
            {
                // Map DTO sang entity
                var questionAnswerEntity = _mapper.Map<QuestionAnswer>(request);

                // Nếu có file, upload và gán FileName mới
                if (!string.IsNullOrEmpty(request.FileName))
                {
                    questionAnswerEntity.FileName = await _cloudinaryService.UploadImageAsync(request.FileName);
                }

                if (request.TopicId != null)
                {
                    Console.WriteLine($"TopicId: {request.TopicId}");
                }

                Console.WriteLine($"TopicIds: {request.TopicId}");
                // Thêm mới câu hỏi/trả lời và liên kết với topic, user
                var addedQuestionAnswer =
                    await _questionsAnswerRepository.AddAsync(questionAnswerEntity, request.TopicId, request.UserId);

                // Gửi thông báo realtime tới client (có thể tùy chỉnh nhóm nếu cần)

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Có câu hỏi mới được thêm vào hệ thống!");

                // --- Gửi thông báo riêng cho người tạo topic ---
                // Lấy thông tin topic để biết người tạo
                var topic = await _topicRepository.GetTopicById(request.TopicId);

                if (topic != null && topic.UserId.HasValue)
                {
                    await _hubContext.Clients.User(topic.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification", "Có bình luận mới trên topic của bạn!");
                }

                // Map entity vừa thêm sang DTO response
                var data = _mapper.Map<QuestionsAnswerResponse>(addedQuestionAnswer);

                // Giả sử bạn có thể lấy topic từ repository (hoặc thông qua service khác)


                return new ApiResponse<QuestionsAnswerResponse?>(0, "Thêm mới thông tin thành công!", data);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hoặc log error nếu cần
                return new ApiResponse<QuestionsAnswerResponse?>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<QuestionsAnswerResponse?>> UpdateAsync(UpdateQuestionsAnswerRequest request,
            int? newTopicId = null)
        {
            try
            {
                // Map DTO sang entity
                var updatedQuestionAnswerEntity = _mapper.Map<QuestionAnswer>(request);

                // Nếu có file mới, upload và gán FileName mới
                if (!string.IsNullOrEmpty(request.FileName))
                {
                    updatedQuestionAnswerEntity.FileName = await _cloudinaryService.UploadImageAsync(request.FileName);
                }

                // Cập nhật bình luận và liên kết với topic (nếu cần)
                var updatedQuestionAnswer =
                    await _questionsAnswerRepository.UpdateAsync(updatedQuestionAnswerEntity, newTopicId);

                // Gửi thông báo chung đến tất cả client (hoặc nhóm cụ thể nếu cần)
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Có câu hỏi được cập nhật trong hệ thống!");

                // --- Gửi thông báo riêng cho người tạo topic ---
                int? topicIdForNotification = newTopicId;
                if (!topicIdForNotification.HasValue)
                {
                    // Lấy thông tin linking từ bảng question_answer_topic_views dựa trên updatedQuestionAnswer.Id
                    var linking =
                        await _questionAnswerTopicViewRepository.GetByIdAsync(updatedQuestionAnswer.Id);
                    topicIdForNotification = linking?.TopicId;
                }

                if (topicIdForNotification.HasValue)
                {
                    var topic = await _topicRepository.GetTopicById(topicIdForNotification.Value);
                    if (topic != null && topic.UserId.HasValue)
                    {
                        await _hubContext.Clients.User(topic.UserId.Value.ToString())
                            .SendAsync("ReceiveNotification", "Có bình luận mới được cập nhật trên topic của bạn!");
                    }
                }
                // ----------------------------------------------------------

                // Map entity đã cập nhật sang DTO response
                var data = _mapper.Map<QuestionsAnswerResponse>(updatedQuestionAnswer);
                return new ApiResponse<QuestionsAnswerResponse?>(0, "Cập nhật thông tin thành công!", data);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionsAnswerResponse?>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }


        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = await _questionsAnswerRepository.DeleteAsync(id);
                if (!result)
                {
                    return new ApiResponse<bool>(1, "Xóa thông tin thất bại!", false);
                }

                return new ApiResponse<bool>(0, "Xóa thông tin thành công!", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionsAnswerResponse>>> GetAllQuestionAnswerByTopicIdAsync(
            int topicId)
        {
            var response = await _questionsAnswerRepository.GetAllQuestionAnswerByTopicIdAsync(topicId);
            if (response == null || !response.Any())
            {
                return new ApiResponse<IEnumerable<QuestionsAnswerResponse>>(1, "Không tìm thấy thông tin!", null);
            }

            var data = _mapper.Map<IEnumerable<QuestionsAnswerResponse>>(response);
            return new ApiResponse<IEnumerable<QuestionsAnswerResponse>>(0, "Lấy danh sách thông tin thành công!",
                data);
        }
    }
}