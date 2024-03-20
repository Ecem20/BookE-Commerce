namespace OrdersAPI.Models.Dto
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public double OrderTotal { get; set; }
        public string? Address { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }
        public string? Status { get; set; }

    }
}
