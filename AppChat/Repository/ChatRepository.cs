using AppChat.Data;
using AppChat.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChat.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;
        public ChatRepository(AppDbContext context) => _context = context;

        public async Task<List<Chat>> GetChatsByUserId(int userId)
        {
            return await _context.Chats
                .Where(c => c.UserAId == userId || c.UserBId == userId)
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();
        }

        public async Task<Chat> GetChatBetweenUsers(int userAId, int userBId)
        {
            return await _context.Chats
                .FirstOrDefaultAsync(c =>
                    (c.UserAId == userAId && c.UserBId == userBId) ||
                    (c.UserAId == userBId && c.UserBId == userAId));
        }

        public async Task<Chat> AddChat(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task UpdateChat(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ArchiveChat(int chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
                return false;

            chat.IsArchived = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnarchiveChat(int chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
                return false;

            chat.IsArchived = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChat(int chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
                return false;

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Chat> GetChatById(int chatId)
        {
            return await _context.Chats.FindAsync(chatId);
        }
    }
}
