using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public double OrderTotal { get; set; }
        public string? Address { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
        public string Status { get; set; }
    }
}
