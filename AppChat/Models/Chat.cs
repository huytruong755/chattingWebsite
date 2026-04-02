using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Chats")]
    public class Chat
    {
        public int Id { get; set; }
        public int UserAId { get; set; }
        public int UserBId { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsArchived { get; set; } = false;

        // Foreign key properties
        [ForeignKey("UserAId")]
        public virtual User? UserA { get; set; }

        [ForeignKey("UserBId")]
        public virtual User? UserB { get; set; }

        // Navigation property for messages
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
