using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;

namespace Project_LMS.Interfaces
{
    public interface IChatMessagesService
    {
        Task<ChatMessageResponse> GetChatMessageById(int id);
        Task<IEnumerable<ChatMessageResponse>> GetAllChatMessages();
        Task AddChatMessage(CreateChatMessageRequest request);
        Task UpdateChatMessage(int id, UpdateChatMessageRequest request);
        Task<bool> DeleteChatMessage(int id);
    }
}