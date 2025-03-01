using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class ChatMessagesService : IChatMessagesService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IMapper _mapper;

        public ChatMessagesService(IChatMessageRepository chatMessagesService, IMapper mapper)
        {
            _chatMessageRepository = chatMessagesService;
            _mapper = mapper;
        }

        public async Task<ChatMessageResponse> GetChatMessageById(int id)
        {
            var chatMessage = await _chatMessageRepository.GetByIdAsync(id);
            return _mapper.Map<ChatMessageResponse>(chatMessage);
        }

        public async Task<IEnumerable<ChatMessageResponse>> GetAllChatMessages()
        {
            var chatMessages = await _chatMessageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ChatMessageResponse>>(chatMessages);
        }

        public async Task AddChatMessage(CreateChatMessageRequest request)
        {
            var chatMessage = _mapper.Map<ChatMessage>(request);
            await _chatMessageRepository.AddAsync(chatMessage);
        }

        public async Task UpdateChatMessage(int id, UpdateChatMessageRequest request)
        {
            var chatMessage = await _chatMessageRepository.GetByIdAsync(id);
            if (chatMessage == null)
            {
                throw new Exception("Chat Message not found");
            }
            _mapper.Map(request, chatMessage);
            await _chatMessageRepository.UpdateAsync(chatMessage);
        }

        public async Task<bool> DeleteChatMessage(int id)
        {
            var chatMessage = await _chatMessageRepository.GetByIdAsync(id);
            if (chatMessage == null)
            {
                throw new Exception("Chat Message not found");
            }
            await _chatMessageRepository.DeleteAsync(id);
            return true;
        }
    }
}