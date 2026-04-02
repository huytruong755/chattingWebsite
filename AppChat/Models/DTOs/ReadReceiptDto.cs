namespace AppChat.Models.DTOs
{
    public class ReadReceiptDto
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public DateTime ReadAt { get; set; }
    }
}
