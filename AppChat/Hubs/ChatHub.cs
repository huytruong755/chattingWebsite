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

    }
}
