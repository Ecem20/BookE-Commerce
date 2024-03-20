namespace OrdersAPI.Models.Dto
{
    public class CartHeadDto
    {
        public int CartHeadId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public double CartTotal { get; set; }
        public string? Address { get; set; }

    }
}
