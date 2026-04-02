namespace AppChat.Models.DTOs
{
    public class BlockedUserDto
    {
        public int Id { get; set; }
        public int BlockedUserId { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime BlockedAt { get; set; }
    }
}
