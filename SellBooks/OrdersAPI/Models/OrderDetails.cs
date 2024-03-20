using OrdersAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }
        public int BookId { get; set; }
        [NotMapped]
        public BookDto? Book { get; set; }
        public int Count { get; set; }
        public string BookName { get; set; }
        public double Price { get; set; }
    }
}
