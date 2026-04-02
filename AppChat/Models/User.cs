using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsOnline { get; set; } = false;
        public DateTime LastSeen { get; set; }

        // Navigation properties
        public virtual ICollection<Chat>? ChatsAsUserA { get; set; }
        public virtual ICollection<Chat>? ChatsAsUserB { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Contact>? ContactsInitiated { get; set; }
        public virtual ICollection<Contact>? ContactsReceived { get; set; }
        public virtual ICollection<ReadReceipt>? ReadReceipts { get; set; }
        public virtual ICollection<BlockedUser>? BlockedUsers { get; set; }
        public virtual ICollection<BlockedUser>? BlockedByUsers { get; set; }
    }
}
