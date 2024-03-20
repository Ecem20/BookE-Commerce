namespace OrdersAPI.Models.Dto
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderHeadId { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string BookName { get; set; }
        public double ProductPrice { get; set; }
    }
}
