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
                    .FirstOrDefaultAsync(ta => ta.Id == replyRequest.TeachingAssignmentId && ta.IsDelete == false);
                if (teachingAssignment == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "TeachingAssignmentId không hợp lệ", null);
                }

                // Kiểm tra ClassId không null
                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "ClassId của phân công giảng dạy không hợp lệ!",
                        null);
                }

                // Kiểm tra ParentCommentId (nếu là trả lời)
                if (replyRequest.ParentCommentId != 0)
                {
                    var parentComment = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == replyRequest.ParentCommentId && qa.IsDelete == false);
                    if (parentComment == null)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1, "Không tìm thấy bình luận gốc", null);
                    }

                    // Kiểm tra ParentComment thuộc cùng TeachingAssignmentId
                    if (parentComment.TeachingAssignmentId != replyRequest.TeachingAssignmentId)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Câu hỏi gốc không thuộc phân công giảng dạy này!", null);
                    }
                }

                // Kiểm tra vai trò user
                var user = await _context.Users.FindAsync(replyRequest.UserId);
                if (user == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "Người dùng không tồn tại!", null);
                }

                // Kiểm tra "Q & A - Học viên hỏi - giảng viên hoặc học viên khác trả lời"
                if (replyRequest.ParentCommentId == 0) // Nếu là câu hỏi mới
                {
                    // Chỉ cho phép học viên (role 3) tạo câu hỏi mới
                    if (user.RoleId != 3)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Chỉ học viên mới được phép đặt câu hỏi mới trong hệ thống Q&A!", null);
                    }
                }
                else // Nếu là câu trả lời
                {
                    // Chỉ cho phép giảng viên (role 2) hoặc học viên khác (role 3) trả lời
                    if (user.RoleId != 2 && user.RoleId != 3)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Chỉ giảng viên hoặc học viên mới được phép trả lời câu hỏi trong hệ thống Q&A!", null);
                    }

                    // Kiểm tra người trả lời không phải là người đặt câu hỏi
                    var parentQuestion = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == replyRequest.ParentCommentId);
                    if (parentQuestion != null && parentQuestion.UserId == replyRequest.UserId)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Bạn không thể tự trả lời câu hỏi của chính mình!", null);
                    }
                }

                // Kiểm tra user có thuộc phân công giảng dạy không (đối với giáo viên)
                bool isUserInTeachingAssignment = true;
                if (user.RoleId == 2) // Nếu là giáo viên
                {
                    isUserInTeachingAssignment = await _context.TeachingAssignments
                        .AnyAsync(ta => ta.Id == replyRequest.TeachingAssignmentId
                                        && ta.UserId == replyRequest.UserId
                                        && ta.IsDelete == false);
                    if (!isUserInTeachingAssignment)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Bạn không được gán vào phân công giảng dạy này để tạo câu hỏi hoặc câu trả lời!", null);
                    }
                }

                // Kiểm tra buổi học (LessonId) nếu có
                if (replyRequest.LessonId.HasValue && replyRequest.LessonId > 0)
                {
                    // Kiểm tra buổi học tồn tại và thuộc về phân công giảng dạy
                    var lesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.Id == replyRequest.LessonId
                                                && l.TeachingAssignmentId == replyRequest.TeachingAssignmentId
                                                && l.IsDelete == false);
                    if (lesson == null)
                    {
                        // Kiểm tra chính xác xem buổi học có tồn tại không
                        var lessonExists = await _context.Lessons
                            .AnyAsync(l => l.Id == replyRequest.LessonId && l.IsDelete == false);

                        if (lessonExists)
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1,
                                "Buổi học không thuộc phân công giảng dạy này!", null);
                        }
                        else
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1,
                                "Buổi học không tồn tại!", null);
                        }
                    }
                }

                string roleName = await GetUserRoleNameAsync(replyRequest.UserId);
                var users = await _context.Users.FindAsync(replyRequest
                    .UserId); // Dòng này không cần thiết vì đã có user ở trên

                // Sử dụng AutoMapper để map từ DTO sang entity QuestionAnswer
                var replyEntity = _mapper.Map<QuestionAnswer>(replyRequest);
                replyEntity.User = user;

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

                var userss = await _context.Users.FindAsync(replyRequest.UserId);
                // Map entity vừa tạo sang DTO phản hồi
                var responseDto = _mapper.Map<QuestionsAnswerResponse>(createdReply);
                responseDto.RoleName = roleName;
                responseDto.Avatar = userss.Image;
                responseDto.FullName = userss.FullName;
                return new ApiResponse<QuestionsAnswerResponse?>(0, "Tạo câu trả lời thành công!", responseDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse<QuestionsAnswerResponse?>(1, $"Lỗi: {ex.Message}", null);
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
                    .Include(qa => qa.User) // Eager load User
                    .FirstOrDefaultAsync(qa => qa.Id == request.Id && qa.IsDelete == false);
                if (existingQuestionAnswer == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1,
                        "Không tìm thấy câu hỏi hoặc câu trả lời, hoặc đã bị xóa", null);
                }

                // 4. Kiểm tra TeachingAssignmentId không bị thay đổi
                if (request.TeachingAssignmentId.HasValue &&
                    request.TeachingAssignmentId != existingQuestionAnswer.TeachingAssignmentId)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1,
                        "TeachingAssignmentId không được thay đổi khi cập nhật!", null);
                }

                // 5. Kiểm tra vai trò user và quyền sở hữu
                var user = await _context.Users.FindAsync(request.UserUpdate.Value);
                if (user == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse>(1, "Người dùng không tồn tại!", null);
                }

                // 6. Kiểm tra TeachingAssignment
                var teachingAssignment = await _context.TeachingAssignments
                    .FirstOrDefaultAsync(ta =>
                        ta.Id == existingQuestionAnswer.TeachingAssignmentId && ta.IsDelete == false);
                if (teachingAssignment == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "TeachingAssignmentId không hợp lệ", null);
                }

                if (teachingAssignment.ClassId == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "ClassId của phân công giảng dạy không hợp lệ!",
                        null);
                }

                // 7. Kiểm tra quy tắc "Q&A - Học viên hỏi - giảng viên hoặc học viên khác trả lời"
                bool isCoreQuestion = existingQuestionAnswer.QuestionsAnswerId == null;

                if (isCoreQuestion) // Nếu là câu hỏi gốc
                {
                    // Chỉ cho phép học viên (role 3) cập nhật câu hỏi của chính mình
                    if (user.RoleId != 3 && user.RoleId != 1 && !(user.RoleId == 2 && teachingAssignment.UserId == user.Id))
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Chỉ học viên (tác giả), giáo viên phụ trách hoặc admin mới được phép cập nhật câu hỏi!", null);
                    }

                    // Nếu không phải tác giả, chỉ giáo viên phụ trách và admin mới có quyền cập nhật
                    if (existingQuestionAnswer.UserId != request.UserUpdate.Value &&
                        !(user.RoleId == 1 || (user.RoleId == 2 && teachingAssignment.UserId == user.Id)))
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Bạn không có quyền cập nhật câu hỏi của người khác!", null);
                    }
                }
                else // Nếu là câu trả lời
                {
                    // Chỉ cho phép giảng viên (role 2) hoặc học viên (role 3) cập nhật câu trả lời của chính mình
                    if (user.RoleId != 2 && user.RoleId != 3 && user.RoleId != 1)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Chỉ giảng viên, học viên (tác giả) hoặc admin mới được phép cập nhật câu trả lời!", null);
                    }

                    // Nếu không phải tác giả, chỉ admin mới có quyền cập nhật
                    if (existingQuestionAnswer.UserId != request.UserUpdate.Value && user.RoleId != 1)
                    {
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            "Bạn không có quyền cập nhật câu trả lời của người khác!", null);
                    }
                }

                // 8. Kiểm tra buổi học (LessonId) nếu có thay đổi
                if (request.LessonId.HasValue && request.LessonId > 0)
                {
                    // Kiểm tra buổi học tồn tại và thuộc về phân công giảng dạy
                    var lesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.Id == request.LessonId
                                                && l.TeachingAssignmentId == existingQuestionAnswer.TeachingAssignmentId
                                                && l.IsDelete == false);
                    if (lesson == null)
                    {
                        // Kiểm tra chính xác xem buổi học có tồn tại không
                        var lessonExists = await _context.Lessons
                            .AnyAsync(l => l.Id == request.LessonId && l.IsDelete == false);

                        if (lessonExists)
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1,
                                "Buổi học không thuộc phân công giảng dạy này!", null);
                        }
                        else
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1,
                                "Buổi học không tồn tại!", null);
                        }
                    }

                    // Kiểm tra học viên có tham gia buổi học này không (chỉ áp dụng cho học viên)
                    if (user.RoleId == 3) // Nếu là học viên
                    {
                        // Kiểm tra học viên có trong danh sách tham gia buổi học online
                        bool isStudentInLesson = await _context.ClassStudentOnlines
                            .AnyAsync(cso => cso.ClassOnline.LessonId == request.LessonId
                                          && cso.UserId == request.UserUpdate.Value
                                          && cso.IsDelete == false);

                        if (!isStudentInLesson)
                        {
                            return new ApiResponse<QuestionsAnswerResponse?>(1,
                                "Bạn không thuộc buổi học này để cập nhật câu hỏi/câu trả lời!", null);
                        }
                    }
                }

                // 9. Kiểm tra quyền sở hữu và phân công giảng dạy (kiểm tra thêm)
                bool hasPermissionToUpdate = false;
                if (user.RoleId == 1) // Admin có quyền cập nhật mọi thứ
                {
                    hasPermissionToUpdate = true;
                }
                else if (user.RoleId == 2) // Giáo viên
                {
                    bool isUserInTeachingAssignment = await _context.TeachingAssignments
                        .AnyAsync(ta => ta.Id == existingQuestionAnswer.TeachingAssignmentId
                                        && ta.UserId == request.UserUpdate.Value
                                        && ta.IsDelete == false);
                    if (isUserInTeachingAssignment)
                    {
                        hasPermissionToUpdate = true;
                    }
                }
                else if (existingQuestionAnswer.UserId == request.UserUpdate.Value) // Người tạo bản ghi
                {
                    hasPermissionToUpdate = true;
                }

                if (!hasPermissionToUpdate)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1,
                        "Bạn không có quyền cập nhật câu hỏi/câu trả lời này!", null);
                }

                // 10. Kiểm tra user có thuộc lớp học không (bỏ qua nếu là admin hoặc đã kiểm tra ở trên cho giáo viên)
                bool isUserInClass = true;
                if (user.RoleId != 1 &&
                    user.RoleId != 2) // Bỏ qua kiểm tra cho admin (RoleId = 1) và giáo viên (RoleId = 2)
                {
                    isUserInClass = await _topicRepository.IsUserInClassAsync(request.UserUpdate.Value,
                        teachingAssignment.ClassId.Value);
                    if (!isUserInClass)
                    {
                        var classInfos = await _context.Classes
                            .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                        return new ApiResponse<QuestionsAnswerResponse?>(1,
                            $"Bạn không thuộc lớp học {classInfos?.Name} để cập nhật câu hỏi này!", null);
                    }
                }

                // Lấy thông tin lớp học (gộp truy vấn để tối ưu)
                var classInfo = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == teachingAssignment.ClassId);
                if (classInfo == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "Lớp học không tồn tại!", null);
                }

                string roleName = await GetUserRoleNameAsync(request.UserUpdate.Value);

                // 11. Map DTO sang entity
                var updatedQuestionAnswerEntity = _mapper.Map<QuestionAnswer>(request);
                updatedQuestionAnswerEntity.Id = existingQuestionAnswer.Id;
                updatedQuestionAnswerEntity.TeachingAssignmentId = existingQuestionAnswer.TeachingAssignmentId;
                updatedQuestionAnswerEntity.UserId = existingQuestionAnswer.UserId;
                updatedQuestionAnswerEntity.CreateAt = existingQuestionAnswer.CreateAt;
                updatedQuestionAnswerEntity.UpdateAt = DateTime.Now;
                updatedQuestionAnswerEntity.IsDelete = existingQuestionAnswer.IsDelete;
                updatedQuestionAnswerEntity.QuestionsAnswerId = existingQuestionAnswer.QuestionsAnswerId;

                // Cập nhật LessonId từ request nếu có, nếu không giữ nguyên giá trị cũ
                updatedQuestionAnswerEntity.LessonId = request.LessonId ?? existingQuestionAnswer.LessonId;

                if (existingQuestionAnswer.User != null)
                {
                    updatedQuestionAnswerEntity.User = updatedQuestionAnswerEntity.User ?? new User();
                    updatedQuestionAnswerEntity.User.FullName = existingQuestionAnswer.User.FullName;
                    updatedQuestionAnswerEntity.User.Image = existingQuestionAnswer.User.Image;
                }
                else
                {
                    updatedQuestionAnswerEntity.User = updatedQuestionAnswerEntity.User ?? new User();
                    updatedQuestionAnswerEntity.User.FullName = "Unknown";
                    updatedQuestionAnswerEntity.User.Image = null;
                }

                // 12. Nếu có file mới, upload và gán FileName mới
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

                // 13. Cập nhật thông tin
                var updatedQuestionAnswer = await _questionsAnswerRepository.UpdateAsync(
                    updatedQuestionAnswerEntity,
                    existingQuestionAnswer.TeachingAssignmentId
                );

                if (updatedQuestionAnswer == null)
                {
                    return new ApiResponse<QuestionsAnswerResponse?>(1, "Cập nhật thất bại, không tìm thấy bản ghi",
                        null);
                }

                // 14. Gửi thông báo
                if (updatedQuestionAnswer.QuestionsAnswerId == null) // Nếu là câu hỏi gốc
                {
                    if (teachingAssignment.UserId.HasValue && teachingAssignment.UserId != request.UserUpdate.Value)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: request.UserUpdate.Value,
                            userId: teachingAssignment.UserId.Value,
                            subject: "Câu hỏi trong lớp đã được cập nhật!",
                            content:
                            $"{user.FullName} đã cập nhật câu hỏi '{updatedQuestionAnswer.Message}' trong lớp {classInfo.Name}.",
                            type: false
                        );
                    }
                }
                else // Nếu là câu trả lời
                {
                    var parentQuestion = await _context.QuestionAnswers
                        .FirstOrDefaultAsync(qa => qa.Id == updatedQuestionAnswer.QuestionsAnswerId);
                    if (parentQuestion != null && parentQuestion.UserId != request.UserUpdate.Value)
                    {
                        await _notificationsService.AddNotificationAsync(
                            senderId: request.UserUpdate.Value,
                            userId: parentQuestion.UserId.Value,
                            subject: "Câu trả lời của bạn đã được cập nhật!",
                            content:
                            $"{user.FullName} đã cập nhật câu trả lời trong câu hỏi '{parentQuestion.Message}' của bạn: {updatedQuestionAnswer.Message}",
                            type: false
                        );
                    }
                }

                // 15. Gửi thông báo realtime
                await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    "Có câu hỏi hoặc câu trả lời được cập nhật trong hệ thống!");

                if (teachingAssignment.UserId.HasValue)
                {
                    await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification",
                            "Có bình luận được cập nhật trong phân công giảng dạy của bạn!");
                }

                // 16. Map và trả về phản hồi
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
                // Kiểm tra userId
                if (userId <= 0)
                {
                    return new ApiResponse<bool>(1, "UserId là bắt buộc và phải lớn hơn 0!", false);
                }

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
                var result = await _questionsAnswerRepository.DeleteAsync(id, userId);
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

                // Gửi thông báo realtime qua SignalR
                if (teachingAssignment.UserId.HasValue)
                {
                    await _hubContext.Clients.User(teachingAssignment.UserId.Value.ToString())
                        .SendAsync("ReceiveNotification",
                            "Có câu hỏi hoặc câu trả lời bị xóa trong phân công giảng dạy của bạn!");
                }

                // Gửi sự kiện SignalR cụ thể để frontend cập nhật
                await _hubContext.Clients.All.SendAsync("OnQuestionAnswerDeleted", id);

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
            int teachingAssignmentId, string tab, int? lessonId = null)
        {
            try
            {
                // Kiểm tra user
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

                // Kiểm tra lessonId nếu được cung cấp
                if (lessonId.HasValue && lessonId > 0 && tab.ToLower() != "topics")
                {
                    var lesson = await _context.Lessons
                        .FirstOrDefaultAsync(l => l.Id == lessonId.Value 
                                               && l.TeachingAssignmentId == teachingAssignmentId 
                                               && l.IsDelete == false);
                    if (lesson == null)
                    {
                        // Kiểm tra chính xác xem buổi học có tồn tại không
                        var lessonExists = await _context.Lessons
                            .AnyAsync(l => l.Id == lessonId.Value && l.IsDelete == false);
                        
                        if (lessonExists)
                        {
                            return new ApiResponse<QuestionsAnswerTabResponse>(1,
                                "Buổi học không thuộc phân công giảng dạy này!", null);
                        }
                        else
                        {
                            return new ApiResponse<QuestionsAnswerTabResponse>(1,
                                "Buổi học không tồn tại!", null);
                        }
                    }
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
                    Views = 0,
                    Replies = 0,
                    Questions = new List<QuestionsAnswerResponse>(),
                    Topics = new List<TopicResponse>()
                };

                // Xử lý theo tab
                switch (tab.ToLower())
                {
                    case "all": // Tab "Tất cả câu hỏi"
                        {
                            // Lấy tất cả câu hỏi gốc và join với Users để lấy Avatar và FullName
                            var questionsQuery = _context.QuestionAnswers
                                .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                             && qa.QuestionsAnswerId == null
                                             && qa.IsDelete == false);
                            
                            // Lọc theo lessonId nếu được cung cấp
                            if (lessonId.HasValue && lessonId.Value > 0)
                            {
                                questionsQuery = questionsQuery.Where(qa => qa.LessonId == lessonId.Value);
                            }
                                     
                            var questions = await questionsQuery
                                .Join(
                                    _context.Users,
                                    qa => qa.UserId,
                                    u => u.Id,
                                    (qa, u) => new { QuestionAnswer = qa, UserAvatar = u.Image, UserFullName = u.FullName })
                                .OrderByDescending(x => x.QuestionAnswer.CreateAt)
                                .ToListAsync();

                            // Lấy tất cả câu trả lời cho các câu hỏi gốc
                            var questionIds = questions.Select(x => x.QuestionAnswer.Id).ToList();
                            var repliesQuery = await _context.QuestionAnswers
                                .Where(qa => questionIds.Contains(qa.QuestionsAnswerId.Value) && qa.IsDelete == false)
                                .Join(
                                    _context.Users,
                                    qa => qa.UserId,
                                    u => u.Id,
                                    (qa, u) => new { Reply = qa, UserAvatar = u.Image, UserFullName = u.FullName })
                                .ToListAsync();

                            // Lấy tất cả ID của câu hỏi và câu trả lời để tính Views
                            var replyIds = repliesQuery.Select(r => r.Reply.Id).ToList();
                            var allIds = questionIds.Concat(replyIds).ToList();

                            // Tính số lượt xem cho từng câu hỏi và câu trả lời
                            var viewsQuery = await _context.QuestionAnswerTopicViews
                                .Where(qatv => allIds.Contains(qatv.QuestionsAnswerId.Value)
                                               && (qatv.IsDelete == false || qatv.IsDelete == null))
                                .GroupBy(qatv => qatv.QuestionsAnswerId)
                                .Select(g => new { QuestionId = g.Key, ViewCount = g.Count() })
                                .ToListAsync();

                            // Tạo dictionary để tra cứu số lượt xem theo QuestionId
                            var viewsDict = viewsQuery.ToDictionary(v => v.QuestionId.Value, v => v.ViewCount);

                            // Tính tổng số lượt xem (Views) cho tab
                            response.Views = viewsQuery.Sum(v => v.ViewCount);

                            // Tính tổng số câu trả lời (Replies)
                            response.Replies = repliesQuery.Count;

                            // Map câu hỏi và câu trả lời sang DTO
                            response.Questions = questions.Select(x =>
                            {
                                var questionResponse = _mapper.Map<QuestionsAnswerResponse>(x.QuestionAnswer);
                                questionResponse.Avatar = x.UserAvatar;
                                questionResponse.FullName = x.UserFullName;

                                // Gán số lượt xem cho câu hỏi
                                questionResponse.Views = viewsDict.ContainsKey(x.QuestionAnswer.Id)
                                    ? viewsDict[x.QuestionAnswer.Id]
                                    : 0;

                                // Lấy danh sách câu trả lời cho câu hỏi này
                                var repliesForQuestion = repliesQuery
                                    .Where(r => r.Reply.QuestionsAnswerId == x.QuestionAnswer.Id)
                                    .Select(r =>
                                    {
                                        var replyResponse = _mapper.Map<QuestionsAnswerResponse>(r.Reply);
                                        replyResponse.Avatar = r.UserAvatar;
                                        replyResponse.FullName = r.UserFullName;
                                        replyResponse.ReplyCount = 0;
                                        replyResponse.Replies = new List<QuestionsAnswerResponse>();
                                        // Gán số lượt xem cho câu trả lời
                                        replyResponse.Views = viewsDict.ContainsKey(r.Reply.Id) ? viewsDict[r.Reply.Id] : 0;
                                        return replyResponse;
                                    })
                                    .ToList();

                                questionResponse.Replies = repliesForQuestion;
                                questionResponse.ReplyCount = repliesForQuestion.Count;

                                return questionResponse;
                            }).ToList();

                            break;
                        }

                    case "answered": // Tab "Đã trả lời"
                        {
                            // Lấy các câu hỏi gốc có ít nhất một câu trả lời
                            var questionsQuery = _context.QuestionAnswers
                                .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                             && qa.QuestionsAnswerId == null
                                             && qa.IsDelete == false)
                                .Where(qa => _context.QuestionAnswers
                                    .Any(reply => reply.QuestionsAnswerId == qa.Id && reply.IsDelete == false));
                            
                            // Lọc theo lessonId nếu được cung cấp
                            if (lessonId.HasValue && lessonId.Value > 0)
                            {
                                questionsQuery = questionsQuery.Where(qa => qa.LessonId == lessonId.Value);
                            }
                            
                            var questions = await questionsQuery
                                .Join(
                                    _context.Users,
                                    qa => qa.UserId,
                                    u => u.Id,
                                    (qa, u) => new { QuestionAnswer = qa, UserAvatar = u.Image, UserFullName = u.FullName })
                                .OrderByDescending(x => x.QuestionAnswer.CreateAt)
                                .ToListAsync();

                            // Lấy tất cả câu trả lời cho các câu hỏi gốc
                            var questionIds = questions.Select(x => x.QuestionAnswer.Id).ToList();
                            var repliesQuery = await _context.QuestionAnswers
                                .Where(qa => questionIds.Contains(qa.QuestionsAnswerId.Value) && qa.IsDelete == false)
                                .Join(
                                    _context.Users,
                                    qa => qa.UserId,
                                    u => u.Id,
                                    (qa, u) => new { Reply = qa, UserAvatar = u.Image, UserFullName = u.FullName })
                                .ToListAsync();

                            // Lấy tất cả ID của câu hỏi và câu trả lời để tính Views
                            var replyIds = repliesQuery.Select(r => r.Reply.Id).ToList();
                            var allIds = questionIds.Concat(replyIds).ToList();

                            // Tính số lượt xem cho từng câu hỏi và câu trả lời
                            var viewsQuery = await _context.QuestionAnswerTopicViews
                                .Where(qatv => allIds.Contains(qatv.QuestionsAnswerId.Value)
                                               && (qatv.IsDelete == false || qatv.IsDelete == null))
                                .GroupBy(qatv => qatv.QuestionsAnswerId)
                                .Select(g => new { QuestionId = g.Key, ViewCount = g.Count() })
                                .ToListAsync();

                            // Tạo dictionary để tra cứu số lượt xem theo QuestionId
                            var viewsDict = viewsQuery.ToDictionary(v => v.QuestionId.Value, v => v.ViewCount);

                            // Tính tổng số lượt xem (Views) cho tab
                            response.Views = viewsQuery.Sum(v => v.ViewCount);

                            // Tính tổng số câu trả lời (Replies)
                            response.Replies = repliesQuery.Count;

                            // Map câu hỏi và câu trả lời sang DTO
                            response.Questions = questions.Select(x =>
                            {
                                var questionResponse = _mapper.Map<QuestionsAnswerResponse>(x.QuestionAnswer);
                                questionResponse.Avatar = x.UserAvatar;
                                questionResponse.FullName = x.UserFullName;

                                // Gán số lượt xem cho câu hỏi
                                questionResponse.Views = viewsDict.ContainsKey(x.QuestionAnswer.Id)
                                    ? viewsDict[x.QuestionAnswer.Id]
                                    : 0;

                                // Lấy danh sách câu trả lời cho câu hỏi này
                                var repliesForQuestion = repliesQuery
                                    .Where(r => r.Reply.QuestionsAnswerId == x.QuestionAnswer.Id)
                                    .Select(r =>
                                    {
                                        var replyResponse = _mapper.Map<QuestionsAnswerResponse>(r.Reply);
                                        replyResponse.Avatar = r.UserAvatar;
                                        replyResponse.FullName = r.UserFullName;
                                        replyResponse.ReplyCount = 0;
                                        replyResponse.Replies = new List<QuestionsAnswerResponse>();
                                        // Gán số lượt xem cho câu trả lời
                                        replyResponse.Views = viewsDict.ContainsKey(r.Reply.Id) ? viewsDict[r.Reply.Id] : 0;
                                        return replyResponse;
                                    })
                                    .ToList();

                                questionResponse.Replies = repliesForQuestion;
                                questionResponse.ReplyCount = repliesForQuestion.Count;

                                return questionResponse;
                            }).ToList();

                            break;
                        }

                    case "near-deadline": // Tab "Gần đến hạn"
                        {
                            // Xác định ngưỡng "gần đến hạn" (ví dụ: 3 ngày trước EndDate)
                            const int nearDeadlineDays = 3;
                            var nearDeadlineThreshold = teachingAssignment.EndDate?.AddDays(-nearDeadlineDays);

                            if (nearDeadlineThreshold == null)
                            {
                                return new ApiResponse<QuestionsAnswerTabResponse>(1,
                                    "EndDate của phân công giảng dạy không hợp lệ!", null);
                            }

                            // Lấy các câu hỏi gốc chưa có câu trả lời và gần đến hạn
                            var questionsQuery = _context.QuestionAnswers
                                .Where(qa => qa.TeachingAssignmentId == teachingAssignmentId
                                             && qa.QuestionsAnswerId == null
                                             && qa.IsDelete == false
                                             && !_context.QuestionAnswers.Any(reply =>
                                                 reply.QuestionsAnswerId == qa.Id && reply.IsDelete == false)
                                             && qa.CreateAt <= nearDeadlineThreshold);
                            
                            // Lọc theo lessonId nếu được cung cấp
                            if (lessonId.HasValue && lessonId.Value > 0)
                            {
                                questionsQuery = questionsQuery.Where(qa => qa.LessonId == lessonId.Value);
                            }
                            
                            var questions = await questionsQuery
                                .Join(
                                    _context.Users,
                                    qa => qa.UserId,
                                    u => u.Id,
                                    (qa, u) => new { QuestionAnswer = qa, UserAvatar = u.Image, UserFullName = u.FullName })
                                .OrderBy(x => x.QuestionAnswer.CreateAt)
                                .ToListAsync();

                            // Tính số lượt xem cho từng câu hỏi
                            var questionIds = questions.Select(x => x.QuestionAnswer.Id).ToList();
                            var viewsQuery = await _context.QuestionAnswerTopicViews
                                .Where(qatv => questionIds.Contains(qatv.QuestionsAnswerId.Value)
                                               && (qatv.IsDelete == false || qatv.IsDelete == null))
                                .GroupBy(qatv => qatv.QuestionsAnswerId)
                                .Select(g => new { QuestionId = g.Key, ViewCount = g.Count() })
                                .ToListAsync();

                            // Tạo dictionary để tra cứu số lượt xem theo QuestionId
                            var viewsDict = viewsQuery.ToDictionary(v => v.QuestionId.Value, v => v.ViewCount);

                            // Tính tổng số lượt xem (Views) cho tab
                            response.Views = viewsQuery.Sum(v => v.ViewCount);

                            // Tính tổng số câu trả lời (Replies) - sẽ là 0 vì đây là các câu hỏi chưa có câu trả lời
                            response.Replies = 0;

                            // Map câu hỏi sang DTO
                            response.Questions = questions.Select(x =>
                            {
                                var questionResponse = _mapper.Map<QuestionsAnswerResponse>(x.QuestionAnswer);
                                questionResponse.Avatar = x.UserAvatar;
                                questionResponse.FullName = x.UserFullName;
                                questionResponse.ReplyCount = 0;
                                questionResponse.Replies = new List<QuestionsAnswerResponse>();

                                // Gán số lượt xem cho câu hỏi
                                questionResponse.Views = viewsDict.ContainsKey(x.QuestionAnswer.Id)
                                    ? viewsDict[x.QuestionAnswer.Id]
                                    : 0;

                                return questionResponse;
                            }).ToList();

                            break;
                        }

                    case "topics": // Tab "Topics" - không sử dụng lessonId
                        {
                            // Lấy danh sách topic gốc (TopicId == null) và join với Users để lấy Avatar và FullName
                            var topicsQuery = await _context.Topics
                                .Where(t => t.TeachingAssignmentId == teachingAssignmentId
                                            && t.TopicId == null
                                            && t.IsDelete == false)
                                .Join(
                                    _context.Users,
                                    t => t.UserId,
                                    u => u.Id,
                                    (t, u) => new { Topic = t, UserAvatar = u.Image, UserFullName = u.FullName })
                                .OrderByDescending(x => x.Topic.CreateAt)
                                .ToListAsync();

                            // Lấy tất cả bình luận (sub-topics) cho các topic gốc
                            var topicIds = topicsQuery.Select(x => x.Topic.Id).ToList();
                            var commentsQuery = await _context.Topics
                                .Where(t => topicIds.Contains(t.TopicId.Value) && t.IsDelete == false)
                                .Join(
                                    _context.Users,
                                    t => t.UserId,
                                    u => u.Id,
                                    (t, u) => new { Comment = t, UserAvatar = u.Image, UserFullName = u.FullName })
                                .ToListAsync();

                            // Lấy tất cả ID của topic và bình luận để tính Views
                            var commentIds = commentsQuery.Select(c => c.Comment.Id).ToList();
                            var allTopicIds = topicIds.Concat(commentIds).ToList();

                            // Tính số lượt xem cho từng topic và bình luận
                            var viewsQuery = await _context.QuestionAnswerTopicViews
                                .Where(qatv => qatv.TopicId.HasValue && allTopicIds.Contains(qatv.TopicId.Value)
                                                                     && (qatv.IsDelete == false || qatv.IsDelete == null))
                                .GroupBy(qatv => qatv.TopicId)
                                .Select(g => new { TopicId = g.Key, ViewCount = g.Count() })
                                .ToListAsync();

                            // Tạo dictionary để tra cứu số lượt xem theo TopicId
                            var viewsDict = viewsQuery.ToDictionary(v => v.TopicId.Value, v => v.ViewCount);

                            // Map sang DTO và gán Avatar, FullName
                            response.Topics = topicsQuery.Select(x =>
                            {
                                var topicResponse = _mapper.Map<TopicResponse>(x.Topic);
                                topicResponse.Avatar = x.UserAvatar;
                                topicResponse.FullName = x.UserFullName;

                                // Gán số lượt xem cho topic
                                topicResponse.Views = viewsDict.ContainsKey(x.Topic.Id) ? viewsDict[x.Topic.Id] : 0;
                                topicResponse.Replies = commentsQuery.Count(c => c.Comment.TopicId == topicResponse.Id);

                                // Lấy danh sách bình luận cho topic này
                                topicResponse.Comments = commentsQuery
                                    .Where(c => c.Comment.TopicId == topicResponse.Id)
                                    .Select(c =>
                                    {
                                        var commentResponse = _mapper.Map<TopicResponse>(c.Comment);
                                        commentResponse.Avatar = c.UserAvatar;
                                        commentResponse.FullName = c.UserFullName;
                                        // Gán số lượt xem cho bình luận
                                        commentResponse.Views =
                                            viewsDict.ContainsKey(c.Comment.Id) ? viewsDict[c.Comment.Id] : 0;
                                        commentResponse.Replies = 0; // Bình luận không có bình luận con
                                        commentResponse.Comments = new List<TopicResponse>();
                                        return commentResponse;
                                    })
                                    .ToList();

                                return topicResponse;
                            }).ToList();

                            // Tính tổng số lượt xem (Views) và câu trả lời (Replies) cho tất cả topics
                            response.Views = response.Topics.Sum(t => t.Views);
                            response.Replies = response.Topics.Sum(t => t.Replies);

                            break;
                        }

                    default:
                        return new ApiResponse<QuestionsAnswerTabResponse>(1, "Tab không hợp lệ!", null);
                }

                // Gán RoleName cho từng câu hỏi, câu trả lời và topic
                foreach (var question in response.Questions)
                {
                    if (question.UserId.HasValue)
                    {
                        question.RoleName = await GetUserRoleNameAsync(question.UserId.Value);
                    }

                    foreach (var reply in question.Replies)
                    {
                        if (reply.UserId.HasValue)
                        {
                            reply.RoleName = await GetUserRoleNameAsync(reply.UserId.Value);
                        }
                    }
                }

                foreach (var topic in response.Topics)
                {
                    if (topic.UserId.HasValue)
                    {
                        topic.RoleName = await GetUserRoleNameAsync(topic.UserId.Value);
                    }

                    foreach (var comment in topic.Comments)
                    {
                        if (comment.UserId.HasValue)
                        {
                            comment.RoleName = await GetUserRoleNameAsync(comment.UserId.Value);
                        }
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