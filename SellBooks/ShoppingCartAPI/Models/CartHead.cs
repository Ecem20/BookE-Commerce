using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartAPI.Models
{
    public class CartHead
    {
        [Key]
        public int CartHeadId { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }

        [NotMapped]
        public double CartTotal { get; set; }
    }
}
