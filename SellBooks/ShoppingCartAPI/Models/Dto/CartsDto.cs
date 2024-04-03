namespace ShoppingCartAPI.Models.Dto
{
    public class CartsDto
    {
        public CartHeadDto CartHead { get; set; }
        public IEnumerable<CartDetailDto>? CartDetail { get; set; }
    }
}