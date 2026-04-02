using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Messages")]
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string? Content { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public string Status { get; set; } = "sent"; // sent, delivered, read
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Foreign key properties
        [ForeignKey("ChatId")]
        public virtual Chat? Chat { get; set; }

        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }

        // Navigation property for read receipts
        public virtual ICollection<ReadReceipt>? ReadReceipts { get; set; }
    }
}

