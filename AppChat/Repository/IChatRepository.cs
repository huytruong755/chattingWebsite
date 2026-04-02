using AppChat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public interface IChatRepository
    {
        Task<List<Chat>> GetChatsByUserId(int userId);
        Task<Chat> GetChatBetweenUsers(int userAId, int userBId);
        Task<Chat> AddChat(Chat chat);
        Task UpdateChat(Chat chat);
        Task<bool> ArchiveChat(int chatId);
        Task<bool> UnarchiveChat(int chatId);
        Task<bool> DeleteChat(int chatId);
        Task<Chat> GetChatById(int chatId);
    }
}
