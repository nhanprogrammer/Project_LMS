using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Helpers;
using Project_LMS.Hubs;
using Project_LMS.Interfaces;
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
        private readonly ApplicationDbContext _context;
        private readonly INotificationsService _notificationsService;

        public QuestionsAnswersService(IQuestionsAnswerRepository questionsAnswerRepository,
            ICloudinaryService cloudinaryService, IMapper mapper,
            ITopicRepository topicRepository, IQuestionsAnswerTopicViewRepository questionAnswerTopicViewRepository,
            IHubContext<RealtimeHub> hubContext, ApplicationDbContext context,
            INotificationsService notificationsService)
        {
            _questionsAnswerRepository = questionsAnswerRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _topicRepository = topicRepository;
            _questionAnswerTopicViewRepository = questionAnswerTopicViewRepository;
            _hubContext = hubContext;
            _context = context;
            _notificationsService = notificationsService;
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

        public async Task<ApiResponse<QuestionsAnswerResponse?>> AddAsync(CreateQuestionsAnswerRequest replyRequest)
        {
            try
            {
                // Kiểm tra TeachingAssignmentId
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == replyRequest.TeachingAssignmentId);
                if (teachingAssignment == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "TeachingAssignmentId không hợp lệ", null);
                }

                // Kiểm tra ClassId không null
                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "ClassId của phân công giảng dạy không hợp lệ!",
                        null);
                }

                // Kiểm tra ParentCommentId (nếu là trả lời)
                if (replyRequest.ParentCommentId != 0)
                {
                    var parentComment = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == replyRequest.ParentCommentId && qa.IsDelete == false);
                    if (parentComment == null)
                    {
                        return new ApiResponse<QuestionsAnswerResponse>(1, "Không tìm thấy bình luận gốc", null);
                    }

                    // Kiểm tra ParentComment thuộc cùng TeachingAssignmentId
                    if (parentComment.TeachingAssignmentId != replyRequest.TeachingAssignmentId)
                    {
                        return new ApiResponse<QuestionsAnswerResponse>(1,
                            "Câu hỏi gốc không thuộc phân công giảng dạy này!", null);
                    }
                }

                // Kiểm tra vai trò user
                var user = await _context.Users.FindAsync(replyRequest.UserId);
                if (user == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "Người dùng không tồn tại!", null);
                }

                // Kiểm tra user có thuộc lớp học không (bỏ qua nếu là admin)
                bool isUserInClass = true;
                if (user.RoleId != 1) // Bỏ qua kiểm tra cho admin (RoleId = 1)
                {
                    isUserInClass =
                        await _topicRepository.IsUserInClassAsync(replyRequest.UserId,
                            teachingAssignment.ClassId.Value);
                    var classMembers = await _context.Classes
                        .Where(c => c.Id == teachingAssignment.ClassId)
                        .FirstOrDefaultAsync();
                    if (!isUserInClass)
                    {
                        return new ApiResponse<QuestionsAnswerResponse>(1,
                            $"Bạn không thuộc lớp học {classMembers?.Name} để bình luận câu hỏi này!", null);
                    }
                }

                string roleName = await GetUserRoleNameAsync(replyRequest.UserId);

                // Sử dụng AutoMapper để map từ DTO sang entity QuestionAnswer
                var replyEntity = _mapper.Map<QuestionAnswer>(replyRequest);

                if (!string.IsNullOrEmpty(replyRequest.FileName))
                {
                    try
                    {
                        replyEntity.FileName = await _cloudinaryService.UploadImageAsync(replyRequest.FileName);
                        if (string.IsNullOrWhiteSpace(replyEntity.FileName))
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1, "Tải file lên thất bại", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1, $"Lỗi tải file: {ex.Message}", null);
                    }
                }

                // Nếu ParentCommentId = 0, đây là câu hỏi gốc, đặt QuestionsAnswerId là null
                if (replyRequest.ParentCommentId == 0)
                {
                    replyEntity.QuestionsAnswerId = null;
                }

                // Tạo bản ghi trong question_answers
                var createdReply = await _questionsAnswerRepository.AddAsync(
                    replyEntity,
                    replyRequest.TeachingAssignmentId
                );

                // Gửi thông báo
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (replyRequest.ParentCommentId == 0) // Nếu là câu hỏi gốc
                {
                    // Gửi thông báo cho giáo viên (nếu người tạo không phải là giáo viên)
                    if (teachingAssignment.UserId.HasValue && teachingAssignment.UserId != replyRequest.UserId)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: replyRequest.UserId, // Người tạo câu hỏi
                            userId: teachingAssignment.UserId.Value,
                            subject: "Có câu hỏi mới trong lớp!",
                            content:
                            $"{user.FullName} đã tạo câu hỏi '{replyRequest.Message}' trong lớp {classInfo?.Name}.",
                            type: false // Thông báo người dùng
                        );
                    }
                }
                else // Nếu là câu trả lời
                {
                    var parentQuestion = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == replyRequest.ParentCommentId);
                    if (parentQuestion != null && parentQuestion.UserId != replyRequest.UserId)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: replyRequest.UserId, // Người trả lời
                            userId: parentQuestion.UserId.Value, // Người tạo câu hỏi gốc
                            subject: "Có câu trả lời mới cho câu hỏi của bạn!",
                            content:
                            $"{user.FullName} đã trả lời câu hỏi '{parentQuestion.Message}' của bạn: {replyRequest.Message}",
                            type: false // Thông báo người dùng
                        );
                    }
                }

                // Gửi thông báo realtime đến giáo viên liên quan
                if (teachingAssignment.UserId.HasValue)
                {
                    await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification", "Có câu trả lời mới trong phân công giảng dạy của bạn!");
                }

                // Map entity vừa tạo sang DTO phản hồi
                var responseDto = _mapper.Map<QuestionsAnswerResponse>(createdReply);
                responseDto.RoleName = roleName;
                return new ApiResponse<QuestionsAnswerResponse?>(0, "Tạo câu trả lời thành công!", responseDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionsAnswerResponse>(1, $"Lỗi: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<QuestionsAnswerResponse?>> UpdateAsync(UpdateQuestionsAnswerRequest request)
        {
            try
            {
                // 1. Kiểm tra Id của bản ghi cần cập nhật
                if (request.Id <= 0)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "Id là bắt buộc và phải lớn hơn 0", null);
                }

                // 2. Kiểm tra UserUpdate
                if (!request.UserUpdate.HasValue || request.UserUpdate <= 0)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "UserUpdate là bắt buộc và phải lớn hơn 0",
                        null);
                }

                // 3. Tìm bản ghi hiện tại trong QuestionAnswers
                var existingQuestionAnswer = await _context.QuestionAnswers
                    .FirstOrDefaultAsync(qa => qa.Id == request.Id && qa.IsDelete == false);
                if (existingQuestionAnswer == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1,
                        "Không tìm thấy câu hỏi hoặc câu trả lời, hoặc đã bị xóa", null);
                }

                // 4. Kiểm tra vai trò user và quyền sở hữu
                var user = await _context.Users.FindAsync(request.UserUpdate.Value);
                if (user == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "Người dùng không tồn tại!", null);
                }

                // Kiểm tra quyền sở hữu: Chỉ người tạo hoặc giáo viên được phép cập nhật
                if (user.RoleId != 2 && existingQuestionAnswer.UserId != request.UserUpdate.Value)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1,
                        "Bạn không có quyền cập nhật câu hỏi/câu trả lời này!", null);
                }

                // 5. Kiểm tra user có thuộc lớp học không (bỏ qua nếu là admin)
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == existingQuestionAnswer.TeachingAssignmentId);
                if (teachingAssignment == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "TeachingAssignmentId không hợp lệ", null);
                }

                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "ClassId của phân công giảng dạy không hợp lệ!",
                        null);
                }

                bool isUserInClass = true;
                if (user.RoleId != 1) // Bỏ qua kiểm tra cho admin (RoleId = 1)
                {
                    isUserInClass = await _topicRepository.IsUserInClassAsync(request.UserUpdate.Value,
                        teachingAssignment.ClassId.Value);
                    var classMembers = await _context.Classes
                        .Where(c => c.Id == teachingAssignment.ClassId)
                        .FirstOrDefaultAsync();
                    if (!isUserInClass)
                    {
                        return new ApiResponse<QuestionsAnswerResponse>(1,
                            $"Bạn không thuộc lớp học {classMembers?.Name} để cập nhật câu hỏi này!", null);
                    }
                }

                string roleName = await GetUserRoleNameAsync(request.UserUpdate.Value);

                // 6. Map DTO sang entity
                var updatedQuestionAnswerEntity = _mapper.Map<QuestionAnswer>(request);

                // 7. Nếu có file mới, upload và gán FileName mới
                if (!string.IsNullOrEmpty(request.FileName))
                {
                    try
                    {
                        updatedQuestionAnswerEntity.FileName =
                            await _cloudinaryService.UploadImageAsync(request.FileName);
                        if (string.IsNullOrWhiteSpace(updatedQuestionAnswerEntity.FileName))
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1, "Tải file lên thất bại", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1, $"Lỗi tải file: {ex.Message}", null);
                    }
                }
                else
                {
                    updatedQuestionAnswerEntity.FileName = existingQuestionAnswer.FileName;
                }

                // 8. Cập nhật thông tin
                var updatedQuestionAnswer = await _questionsAnswerRepository.UpdateAsync(
                    updatedQuestionAnswerEntity,
                    request.TeachingAssignmentId
                );

                if (updatedQuestionAnswer == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "Cập nhật thất bại, không tìm thấy bản ghi",
                        null);
                }

                // 9. Gửi thông báo
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (updatedQuestionAnswer.QuestionsAnswerId == null) // Nếu là câu hỏi gốc
                {
                    // Gửi thông báo cho giáo viên (nếu người cập nhật không phải là giáo viên)
                    if (teachingAssignment.UserId.HasValue && teachingAssignment.UserId != request.UserUpdate.Value)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: request.UserUpdate.Value, // Người cập nhật câu hỏi
                            userId: teachingAssignment.UserId.Value,
                            subject: "Câu hỏi trong lớp đã được cập nhật!",
                            content:
                            $"{user.FullName} đã cập nhật câu hỏi '{updatedQuestionAnswer.Message}' trong lớp {classInfo?.Name}.",
                            type: false // Thông báo người dùng
                        );
                    }
                }
                else // Nếu là câu trả lời
                {
                    // Gửi thông báo cho người tạo câu hỏi gốc
                    var parentQuestion = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == updatedQuestionAnswer.QuestionsAnswerId);
                    if (parentQuestion != null && parentQuestion.UserId != request.UserUpdate.Value)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: request.UserUpdate.Value, // Người cập nhật câu trả lời
                            userId: parentQuestion.UserId.Value, // Người tạo câu hỏi gốc
                            subject: "Câu trả lời của bạn đã được cập nhật!",
                            content:
                            $"{user.FullName} đã cập nhật câu trả lời trong câu hỏi '{parentQuestion.Message}' của bạn: {updatedQuestionAnswer.Message}",
                            type: false // Thông báo người dùng
                        );
                    }
                }

                // 10. Gửi thông báo realtime
                await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    "Có câu hỏi hoặc câu trả lời được cập nhật trong hệ thống!");

                int? teachingAssignmentIdForNotification = request.TeachingAssignmentId;
                if (!teachingAssignmentIdForNotification.HasValue &&
                    updatedQuestionAnswer?.TeachingAssignmentId.HasValue == true)
                {
                    teachingAssignmentIdForNotification = updatedQuestionAnswer.TeachingAssignmentId;
                }

                if (teachingAssignmentIdForNotification.HasValue)
                {
                    var teachingAssignmentForNotification = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentIdForNotification.Value);
                    if (teachingAssignmentForNotification != null && teachingAssignmentForNotification.UserId.HasValue)
                    {
                        await _hubContext.Clients.User(teachingAssignmentForNotification.UserId.Value.ToString())
                            .SendAsync("ReceiveNotification",
                                "Có bình luận được cập nhật trong phân công giảng dạy của bạn!");
                    }
                }

                // 11. Map và trả về phản hồi
                var responseDto = _mapper.Map<QuestionsAnswerResponse>(updatedQuestionAnswer);
                responseDto.RoleName = roleName;
                return new ApiResponse<QuestionsAnswerResponse?>(0, "Cập nhật thông tin thành công!", responseDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionsAnswerResponse?>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            try
            {
                // Tìm bản ghi cần xóa
                var questionAnswer = await _context.QuestionAnswers
                    .FirstOrDefaultAsync(qa => qa.Id == id && qa.IsDelete == false);
                if (questionAnswer == null)
                {
                    return new ApiResponse<bool>(1, "Câu hỏi hoặc câu trả lời không tồn tại hoặc đã bị xóa!", false);
                }

                // Kiểm tra vai trò user và quyền sở hữu
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponse<bool>(1, "Người dùng không tồn tại!", false);
                }

                // Kiểm tra quyền sở hữu: Chỉ người tạo hoặc giáo viên được phép xóa
                if (user.RoleId != 2 && questionAnswer.UserId != userId)
                {
                    return new ApiResponse<bool>(1, "Bạn không có quyền xóa câu hỏi/câu trả lời này!", false);
                }

                // Lấy thông tin phân công giảng dạy
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == questionAnswer.TeachingAssignmentId);
                if (teachingAssignment == null || teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<bool>(1, "Phân công giảng dạy không hợp lệ!", false);
                }

                // Xóa bản ghi
                var result = await _questionsAnswerRepository.DeleteAsync(id);
                if (!result)
                {
                    return new ApiResponse<bool>(1, "Xóa thông tin thất bại!", false);
                }

                // Gửi thông báo
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (questionAnswer.QuestionsAnswerId == null) // Nếu là câu hỏi gốc
                {
                    // Gửi thông báo cho giáo viên (nếu người xóa không phải là giáo viên)
                    if (teachingAssignment.UserId.HasValue && teachingAssignment.UserId != userId)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: userId, // Người xóa
                            userId: teachingAssignment.UserId.Value,
                            subject: "Câu hỏi trong lớp đã bị xóa!",
                            content:
                            $"{user.FullName} đã xóa câu hỏi '{questionAnswer.Message}' trong lớp {classInfo?.Name}.",
                            type: false // Thông báo người dùng
                        );
                    }
                }
                else // Nếu là câu trả lời
                {
                    // Gửi thông báo cho người tạo câu hỏi gốc
                    var parentQuestion = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == questionAnswer.QuestionsAnswerId);
                    if (parentQuestion != null && parentQuestion.UserId != userId)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: userId, // Người xóa
                            userId: parentQuestion.UserId.Value, // Người tạo câu hỏi gốc
                            subject: "Câu trả lời trong câu hỏi của bạn đã bị xóa!",
                            content:
                            $"{user.FullName} đã xóa câu trả lời trong câu hỏi '{parentQuestion.Message}' của bạn.",
                            type: false // Thông báo người dùng
                        );
                    }
                }

                // Gửi thông báo realtime
                if (teachingAssignment.UserId.HasValue)
                {
                    await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification",
                            "Có câu hỏi hoặc câu trả lời bị xóa trong phân công giảng dạy của bạn!");
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

        public async Task<ApiResponse<ClassMembersWithStatsResponse>> GetClassMembersByTeachingAssignmentAsync(
            int teachingAssignmentId, string? searchTerm = null)
        {
            try
            {
                var result =
                    await _questionsAnswerRepository.GetClassMembersByTeachingAssignmentAsync(teachingAssignmentId,
                        searchTerm);
                return new ApiResponse<ClassMembersWithStatsResponse>(0, "Lấy danh sách thành viên thành công!",
                    result);
            }
            catch (ArgumentException ex)
            {
                return new ApiResponse<ClassMembersWithStatsResponse>(1, ex.Message, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ClassMembersWithStatsResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<TeachingAssignmentStudentsResponse>> GetStudentsByTeachingAssignmentAsync(
            int teachingAssignmentId)
        {
            try
            {
                var studentsResponse =
                    await _questionsAnswerRepository.GetTeachingAssignmentStudentsAsync(teachingAssignmentId);

                if (studentsResponse == null || !studentsResponse.Students.Any())
                {
                    return new ApiResponse<TeachingAssignmentStudentsResponse>(1,
                        "Không tìm thấy sinh viên nào trong phân công giảng dạy này.", null);
                }

                return new ApiResponse<TeachingAssignmentStudentsResponse>(0, "Lấy danh sách sinh viên thành công!",
                    studentsResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<TeachingAssignmentStudentsResponse>(2, $"Lỗi hệ thống: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<QuestionsAnswerTabResponse>> GetQuestionsAnswersByTabAsync(int userId,
            int teachingAssignmentId, string tab)
        {
            try
            {
                // kiểm tra user
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponse<QuestionsAnswerTabResponse>(1, "Người dùng không tồn tại!", null);
                }

                // Kiểm tra teachingAssignment
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == teachingAssignmentId && ta.IsDelete == false);
                if (teachingAssignment == null)
                {
                    return new ApiResponse<QuestionsAnswerTabResponse>(1, "Phân công giảng dạy không tồn tại!", null);
                }

                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<QuestionsAnswerTabResponse>(1,
                        "ClassId của phân công giảng dạy không hợp lệ!", null);
                }

                // Kiểm tra user có quyền truy cập TeachingAssignment không
                bool isUserInClass = true;
                if (user.RoleId != 1)
                {
                    isUserInClass = await _topicRepository.IsUserInClassAsync(userId, teachingAssignment.ClassId.Value);
                    if (!isUserInClass)
                    {
                        var classMembers = await _context.Classes
                            .Where(c => c.Id == teachingAssignment.ClassId)
                            .FirstOrDefaultAsync();
                        return new ApiResponse<QuestionsAnswerTabResponse>(1,
                            $"Bạn không thuộc lớp học {classMembers?.Name} để xem dữ liệu này!", null);
                    }
                }

                var response = new QuestionsAnswerTabResponse
                {
                    Questions = new List<QuestionsAnswerResponse>(),
                    Topics = new List<TopicResponse>()
                };

                // Xử lý theo tab
                switch (tab.ToLower())
                {
                    case "all": // Tab "Tất cả câu hỏi"
                    {
                        // Lấy tất cả câu hỏi gốc (QuestionsAnswerId = null)
                        var questions = await _context.QuestionAnswers
                            .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                         && qa.QuestionsAnswerId == null
                                         && qa.IsDelete == false)
                            .OrderByDescending(qa => qa.CreateAt)
                            .ToListAsync();

                        // Tổng số lượng
                        response.TotalCount = questions.Count;

                        // Map sang DTO
                        response.Questions = _mapper.Map<List<QuestionsAnswerResponse>>(questions);
                        break;
                    }

                    case "answered": // Tab "Đã trả lời"
                    {
                        // Lấy các câu hỏi gốc có ít nhất một câu trả lời
                        var questions = await _context.QuestionAnswers
                            .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                         && qa.QuestionsAnswerId == null
                                         && qa.IsDelete == false)
                            .Where(qa => _context.QuestionAnswers
                                .Any(reply => reply.QuestionsAnswerId == qa.Id && reply.IsDelete == false))
                            .OrderByDescending(qa => qa.CreateAt)
                            .ToListAsync();

                        // Tổng số lượng
                        response.TotalCount = questions.Count;

                        // Map sang DTO
                        response.Questions = _mapper.Map<List<QuestionsAnswerResponse>>(questions);
                        break;
                    }

                    case "near-deadline": // Tab "Gần đến hạn"
                    {
                        var questions = await _context.QuestionAnswers
                            .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                         && qa.QuestionsAnswerId == null // Chỉ lấy câu hỏi gốc
                                         && qa.IsDelete == false
                                         && !_context.QuestionAnswers.Any(reply =>
                                             reply.QuestionsAnswerId == qa.Id &&
                                             reply.IsDelete == false)) // Chưa có câu trả lời
                            .OrderBy(qa => qa.CreateAt) // Sắp xếp theo thời gian tạo (tăng dần)
                            .ToListAsync();
                        questions = questions.Where(q =>
                                q.CreateAt.HasValue && q.CreateAt.Value.AddHours(24) >= DateTime.UtcNow)
                            .ToList(); // Tổng số lượng
                        response.TotalCount = questions.Count;

                        // Map sang DTO
                        response.Questions = _mapper.Map<List<QuestionsAnswerResponse>>(questions);
                        break;
                    }

                    case "topics": // Tab "Topics"
                    {
                        // Lấy danh sách topic
                        var topics = await _context.Topics
                            .Where(t => t.TeachingAssignmentId == teachingAssignmentId
                                        && t.TopicId == null
                                        && t.IsDelete == false)
                            .OrderByDescending(t => t.CreateAt)
                            .ToListAsync();

                        // Tổng số lượng
                        response.TotalCount = topics.Count;

                        // Map sang DTO
                        response.Topics = _mapper.Map<List<TopicResponse>>(topics);

                        // Tính views và replies cho topics
                        foreach (var topic in response.Topics)
                        {
                            topic.Views = await _context.QuestionAnswerTopicViews
                                .CountAsync(tv =>
                                    tv.TopicId == topic.Id && (tv.IsDelete == false || tv.IsDelete == null));
                            topic.Replies = await _context.Topics
                                .CountAsync(t => t.TopicId == topic.Id && t.IsDelete == false);
                            topic.Comments = new List<TopicResponse>();
                        }

                        break;
                    }

                    default:
                        return new ApiResponse<QuestionsAnswerTabResponse>(1, "Tab không hợp lệ!", null);
                }

                foreach (var question in response.Questions)
                {
                    if (question.UserId != null)
                    {
                        question.RoleName = await GetUserRoleNameAsync(question.UserId.Value);
                    }
                }

                return new ApiResponse<QuestionsAnswerTabResponse>(0, "Lấy dữ liệu thành công!", response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionsAnswerTabResponse>(1, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<bool>> SendUserMessageAsync(int senderId, int receiverId, string message)
        {
            try
            {
                // Kiểm tra người gửi và người nhận
                var sender = await _context.Users.FindAsync(senderId);
                var receiver = await _context.Users.FindAsync(receiverId);
                if (sender == null || receiver == null)
                {
                    return new ApiResponse<bool>(1, "Người gửi hoặc người nhận không tồn tại!", false);
                }

                // Gửi thông báo người dùng
                await _notificationsService.AddNotificationAsync(
                    senderId: senderId, // Người gửi
                    userId: receiverId, // Người nhận
                    subject: $"Tin nhắn từ {sender.FullName}",
                    content: message,
                    type: false // Thông báo người dùng
                );

                // Gửi thông báo realtime
                await _hubContext.Clients.User(receiverId.ToString())
                    .SendAsync("ReceiveNotification", $"Bạn có tin nhắn mới từ {sender.FullName}: {message}");

                return new ApiResponse<bool>(0, "Gửi tin nhắn thành công!", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(1, $"Có lỗi xảy ra: {ex.Message}", false);
            }
        }

        private async Task<string> GetUserRoleNameAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.Role == null)
            {
                return "Unknown";
            }

            return user.Role.Name;
        }

        public async Task<ApiResponse<QuestionDetailResponse>> GetByIdWithViewAsync(int id, int? userId)
        {
            try
            {
                // Lấy thông tin câu hỏi
                var question = await _context.QuestionAnswers
                    .AsNoTracking()
                    .Where(qa => qa.Id == id && qa.IsDelete == false)
                    .Select(qa => new
                    {
                        qa.Id,
                        qa.Message,
                        UserId = qa.UserId ?? 0, // Handle nullable UserId
                        CreateAt = qa.CreateAt ?? DateTime.MinValue, // Handle nullable CreateAt
                        qa.FileName,
                        qa.TeachingAssignmentId,
                        qa.QuestionsAnswerId,
                        UserFullName = qa.User.FullName,
                        RoleName = qa.User.Role.Name
                    })
                    .FirstOrDefaultAsync();

                if (question == null)
                {
                    return new ApiResponse<QuestionDetailResponse>(1, "Câu hỏi không tồn tại hoặc đã bị xóa.", null);
                }

                // Kiểm tra user có thuộc lớp học không (nếu userId được cung cấp)
                if (userId.HasValue)
                {
                    var teachingAssignment = await _context.TeachingAssignments
                        .FirstOrDefaultAsync(ta => ta.Id == question.TeachingAssignmentId);
                    if (teachingAssignment == null || teachingAssignment.ClassId == null)
                    {
                        return new ApiResponse<QuestionDetailResponse>(2, "Phân công giảng dạy không hợp lệ.", null);
                    }

                    bool isUserInClass =
                        await _topicRepository.IsUserInClassAsync(userId.Value, teachingAssignment.ClassId.Value);
                    var classMembers = await _context.Classes.Where(c => c.Id == teachingAssignment.ClassId)
                        .FirstOrDefaultAsync();
                    if (!isUserInClass)
                    {
                        return new ApiResponse<QuestionDetailResponse>(2,
                            $"Bạn không thuộc lớp học {classMembers?.Name} để xem câu hỏi này!", null);
                    }

                    // Ghi lượt xem (nếu đây là câu hỏi gốc và user chưa xem)
                    if (question.QuestionsAnswerId == null)
                    {
                        var existingView = await _context.QuestionAnswerTopicViews
                            .AnyAsync(qatv => qatv.QuestionsAnswerId == id
                                              && qatv.UserId == userId.Value
                                              && qatv.IsDelete == false);

                        if (!existingView)
                        {
                            var view = new QuestionAnswerTopicView
                            {
                                QuestionsAnswerId = id,
                                UserId = userId.Value,
                                IsDelete = false,
                                CreateAt = DateTime.UtcNow
                            };
                            _context.QuestionAnswerTopicViews.Add(view);
                            await _context.SaveChangesAsync();

                            // Gửi thông báo realtime (nếu cần)
                            if (teachingAssignment.UserId.HasValue)
                            {
                                await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                                    .SendAsync("ReceiveNotification",
                                        $"Câu hỏi (ID: {id}) đã được xem bởi user {userId.Value}.");
                            }
                        }
                    }
                }

                // Lấy danh sách câu trả lời (nếu đây là câu hỏi gốc)
                var answers = new List<AnswerInfoResponse>();
                if (question.QuestionsAnswerId == null)
                {
                    answers = await _context.QuestionAnswers
                        .AsNoTracking()
                        .Where(qa => qa.QuestionsAnswerId == id && qa.IsDelete == false)
                        .Select(qa => new AnswerInfoResponse
                        {
                            Id = qa.Id,
                            Content = qa.Message,
                            FileName = qa.FileName,
                            UserId = qa.UserId ?? 0, // Handle nullable UserId
                            UserFullName = qa.User.FullName,
                            RoleName = qa.User.Role.Name,
                            CreateAt = qa.CreateAt ?? DateTime.MinValue // Handle nullable CreateAt
                        })
                        .ToListAsync();
                }

                // Trả về thông tin chi tiết
                var responseData = new QuestionDetailResponse
                {
                    Id = question.Id,
                    Content = question.Message,
                    FileName = question.FileName,
                    UserId = question.UserId,
                    UserFullName = question.UserFullName,
                    RoleName = question.RoleName,
                    CreateAt = question.CreateAt,
                    Answers = answers
                };

                return new ApiResponse<QuestionDetailResponse>(0, "Lấy chi tiết câu hỏi thành công!", responseData);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionDetailResponse>(2, $"Lỗi hệ thống: {ex.Message}", null);
            }
        }
    }
}