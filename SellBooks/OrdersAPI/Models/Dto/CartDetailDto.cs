using OrdersAPI.Models.Dto;

namespace OrdersAPI.Models
{
    public class CartDetailDto
    {
        public int CartDetailId { get; set; }
        public int CartHeadId { get; set; }
        public CartHeadDto? CartHead { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
    }
}
