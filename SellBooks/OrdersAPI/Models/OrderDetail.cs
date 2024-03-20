using OrdersAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersAPI.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderHeadId { get; set; }
        [ForeignKey("OrderHeadId")]
        public OrderHead? OrderHead { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string BookName { get; set; }
        public double ProductPrice { get; set; }
    }
}
