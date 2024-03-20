namespace OrdersAPI.Models.Dto
{
    public class OrderHeadDto
    {
        public int OrderHeadId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public double OrderTotal { get; set; }
        public string? Address { get; set; }
        public IEnumerable<OrderDetailDto> OrderDetail { get; set; }
        public string Status { get; set; }
    }
}
