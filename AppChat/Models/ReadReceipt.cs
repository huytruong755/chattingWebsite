using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("ReadReceipts")]
    public class ReadReceipt
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;

        // Foreign key properties
        [ForeignKey("MessageId")]
        public virtual Message? Message { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
