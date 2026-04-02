using System.ComponentModel.DataAnnotations.Schema;

namespace AppChat.Models
{
    [Table("Contacts")]
    public class Contact
    {
        public int Id { get; set; }
        public int UserIdContactA { get; set; }
        public int UserIdContactB { get; set; }

        // Foreign key properties
        [ForeignKey("UserIdContactA")]
        public virtual User? UserA { get; set; }

        [ForeignKey("UserIdContactB")]
        public virtual User? UserB { get; set; }
    }
}
