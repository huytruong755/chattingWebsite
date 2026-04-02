using AppChat.Data;
using AppChat.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AppChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly MessageService _msgService;

        public ChatHub(AppDbContext context, MessageService service)
        {
            _context = context;
            _msgService = service;
        }

        public string UserId => Context.UserIdentifier ?? "NULL";  // Get user id from token
        
        public override async Task OnConnectedAsync()
        {
            try
            {
                var identity = Context.User?.Identity;
                Console.WriteLine($"[UserId from Context]: {UserId}");
                if (identity != null) Console.WriteLine($"IsAuthenticated: {identity.IsAuthenticated}");
                else
                {
                    Console.WriteLine("Context.User.Identity IS NULL");
                    throw new UnauthorizedAccessException("Unauthorized");
                }

                Console.WriteLine("Claims:");
                if (Context.User != null)
                {
                    foreach (var c in Context.User.Claims)
                    {
                        Console.WriteLine($"  {c.Type}: {c.Value}");
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }

                // ===== Update user online status =====
                if (int.TryParse(UserId, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.IsOnline = true;
                        user.LastSeen = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[OnConnected] User {userId} is now ONLINE");
                    }
                }

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                throw new HubException("Error: ", ex);
            }
        }

        // ===== Handle Disconnect =====
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (int.TryParse(UserId, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.IsOnline = false;
                        user.LastSeen = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[OnDisconnected] User {userId} is now OFFLINE");
                    }
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OnDisconnected] Error: {ex.Message}");
                await base.OnDisconnectedAsync(exception);
            }
        }

        public async Task JoinGroup(string chatId)
        {
            Console.WriteLine($"[JoinGroup] called chatId = '{chatId}' on ConnectionId = {Context.ConnectionId}");
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                await MarkMessagesAsRead(int.Parse(chatId));
                Console.WriteLine("Joined group successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in JoinGroup: " + ex);
                throw new HubException("JoinGroup error: " + ex.Message);
            }
        }

        public async Task MarkMessagesAsRead(int chatId)
        {
            int userId = int.Parse(UserId);

            // Lấy messages chưa đọc
            var unreadMessages = await _context.Messages
                .Where(m => m.ChatId == chatId &&
                            m.SenderId != userId &&
                            m.Status.ToLower() == "sent")
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.Status = "read";
                }

                await _context.SaveChangesAsync();
            }

            // Gửi realtime cho chính user (reset UI unread)
            await Clients.User(userId.ToString())
                .SendAsync("MessagesRead", chatId);

            // Gửi realtime cho người gửi (để cập nhật tick “đã đọc”)
            await Clients.Group(chatId.ToString())
                .SendAsync("UpdateMessageStatus", chatId, userId, "read");
        }
        // ===== Message Edit =====
        public async Task EditMessage(int messageId, int chatId, string newContent)
        {
            try
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message == null)
                    throw new Exception("Message not found");

                message.Content = newContent;
                message.EditedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Broadcast to all users in the chat
                await Clients.Group(chatId.ToString())
                    .SendAsync("MessageEdited", new { messageId, newContent, editedAt = message.EditedAt });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditMessage] Error: {ex.Message}");
                throw new HubException($"Error editing message: {ex.Message}");
            }
        }

        // ===== Message Delete =====
        public async Task DeleteMessage(int messageId, int chatId)
        {
            try
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message == null)
                    throw new Exception("Message not found");

                message.IsDeleted = true;
                message.DeletedAt = DateTime.UtcNow;
                message.Content = null;
                await _context.SaveChangesAsync();

                // Broadcast to all users in the chat
                await Clients.Group(chatId.ToString())
                    .SendAsync("MessageDeleted", messageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteMessage] Error: {ex.Message}");
                throw new HubException($"Error deleting message: {ex.Message}");
            }
        }

        // ===== User Typing =====
        public async Task UserStartedTyping(int chatId, string userName)
        {
            try
            {
                int userId = int.Parse(UserId);
                // Broadcast to all users in the chat except sender
                await Clients.Group(chatId.ToString())
                    .SendAsync("UserTyping", userId, userName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserStartedTyping] Error: {ex.Message}");
            }
        }

        // ===== User Stopped Typing =====
        public async Task UserStoppedTyping(int chatId)
        {
            try
            {
                int userId = int.Parse(UserId);
                // Broadcast to all users in the chat
                await Clients.Group(chatId.ToString())
                    .SendAsync("UserStoppedTyping", userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserStoppedTyping] Error: {ex.Message}");
            }
        }

        // ===== Mark Specific Message as Read =====
        public async Task MarkMessageAsRead(int messageId, int userId, int chatId)
        {
            try
            {
                // Check if read receipt already exists
                var existingReceipt = await _context.ReadReceipts
                    .FirstOrDefaultAsync(rr => rr.MessageId == messageId && rr.UserId == userId);

                if (existingReceipt == null)
                {
                    var readReceipt = new Models.ReadReceipt
                    {
                        MessageId = messageId,
                        UserId = userId,
                        ReadAt = DateTime.UtcNow
                    };

                    _context.ReadReceipts.Add(readReceipt);
                    await _context.SaveChangesAsync();
                }

                // Broadcast to all users in the chat
                await Clients.Group(chatId.ToString())
                    .SendAsync("MessageReadByUser", messageId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MarkMessageAsRead] Error: {ex.Message}");
            }
        }
    }
}
