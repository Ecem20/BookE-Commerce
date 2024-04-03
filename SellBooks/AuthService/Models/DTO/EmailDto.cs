namespace SellBooks.Models.DTO
{
    public class EmailDto
    {
        public int Id { get; set; }
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
