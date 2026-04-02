using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("BlockedUsers")]
    public class BlockedUser
    {
        public int Id { get; set; }
        public int BlockerId { get; set; }
        public int BlockedUserId { get; set; }
        public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

        // Foreign key properties
        [ForeignKey("BlockerId")]
        public virtual User? Blocker { get; set; }

        [ForeignKey("BlockedUserId")]
        public virtual User? BlockedUserNav { get; set; }
    }
}
