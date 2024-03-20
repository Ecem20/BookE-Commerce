namespace ShoppingCartAPI.Models.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }

        public double CartTotal { get; set; }
    }
}
