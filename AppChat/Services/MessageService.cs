using AppChat.Models.DTOs;
using AppChat.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _repo;
        public MessageService(IMessageRepository repo) => _repo = repo;

        // Lấy tin nhắn theo chatId
        public async Task<List<MessageDto>> GetMessagesByChatIdAsync(int chatId)
        {
            return await _repo.GetMessagesByChatIdAsync(chatId);
        }

        // Gửi/lưu tin nhắn
        public async Task<MessageDto> SendMessageAsync(int? chatId, int senderId, int receiverId, string? content, string fileType, string? fileUrl)
        {
            return await _repo.SendMessageAsync(chatId, senderId, receiverId, content, fileType, fileUrl);
        }

        // Sửa tin nhắn
        public async Task<MessageDto> EditMessageAsync(int messageId, string content)
        {
            return await _repo.EditMessageAsync(messageId, content);
        }

        // Xóa tin nhắn (soft delete)
        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            return await _repo.DeleteMessageAsync(messageId);
        }

        // Tìm kiếm tin nhắn
        public async Task<List<MessageDto>> SearchMessagesAsync(int chatId, string searchTerm)
        {
            return await _repo.SearchMessagesAsync(chatId, searchTerm);
        }
    }
}

