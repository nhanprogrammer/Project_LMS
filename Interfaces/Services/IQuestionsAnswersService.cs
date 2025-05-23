using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces.Services
{
    public interface IQuestionsAnswersService
    {
        // Lấy danh sách câu hỏi/trả lời với phân trang, trả về DTO response
        Task<ApiResponse<PaginatedResponse<QuestionsAnswerResponse>>> GetAllAsync(PaginationRequest request);

        // Lấy chi tiết của một câu hỏi/trả lời theo id
        Task<ApiResponse<QuestionDetailResponse>> GetByIdWithViewAsync(int id, int? userId);

        // Thêm mới câu hỏi/trả lời dựa trên DTO yêu cầu, service sẽ gọi repository.AddAsync(entity, topicId, userId)
        Task<ApiResponse<QuestionsAnswerResponse?>> AddAsync(CreateQuestionsAnswerRequest request);

        // Cập nhật câu hỏi/trả lời, có thể cập nhật cả topic nếu cần
        Task<ApiResponse<QuestionsAnswerResponse?>> UpdateAsync(UpdateQuestionsAnswerRequest request);

        // Xóa câu hỏi/trả lời theo id
        Task<ApiResponse<bool>> DeleteAsync(int id,int userId);

        // Lấy danh sách câu hỏi/trả lời theo topicId
        Task<ApiResponse<IEnumerable<QuestionsAnswerResponse>>> GetAllQuestionAnswerByTopicIdAsync(int topicId);

        Task<ApiResponse<ClassMembersWithStatsResponse>> GetClassMembersByTeachingAssignmentAsync(
            int teachingAssignmentId, string? searchTerm = null);

        Task<ApiResponse<TeachingAssignmentStudentsResponse>> GetStudentsByTeachingAssignmentAsync(
            int teachingAssignmentId);

        Task<ApiResponse<QuestionsAnswerTabResponse>> GetQuestionsAnswersByTabAsync(int userId,
            int teachingAssignmentId, string tab, int? lessonId = null);
        
        Task<ApiResponse<bool>> SendUserMessageAsync(int senderId, int receiverId, string message);
    }
}