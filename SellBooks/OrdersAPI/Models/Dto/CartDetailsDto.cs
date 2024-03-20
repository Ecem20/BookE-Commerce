using OrdersAPI.Models.Dto;

namespace OrdersAPI.Models
{
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int BookId { get; set; }
        public BookDto? Book { get; set; }
        public int Count { get; set; }
    }
}
