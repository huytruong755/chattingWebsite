using AppChat.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IMessageRepository
    {
        Task<List<MessageDto>> GetMessagesByChatIdAsync(int chatId);

        Task<MessageDto> SendMessageAsync(
            int? chatId,
            int senderId,
            int receiverId,
            string? content,
            string fileType,
            string? fileUrl
        );

        Task<MessageDto> EditMessageAsync(int messageId, string content);

        Task<bool> DeleteMessageAsync(int messageId);

        Task<List<MessageDto>> SearchMessagesAsync(int chatId, string searchTerm);
    }
}
