using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Models.Dto
{
    public class OrderDetailsDto
    {
        public int OrderDetailsId { get; set; }
        public int OrderHeaderId { get; set; }
        public int BookId { get; set; }
        public BookDto? Book { get; set; }
        public int Count { get; set; }
        public string BookName { get; set; }
        public double Price { get; set; }
    }
}
