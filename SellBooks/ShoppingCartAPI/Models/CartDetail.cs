using ShoppingCartAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCartAPI.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        public int CartHeadId { get; set; }
        [ForeignKey("CartHeadId")]
        public CartHead CartHead { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}
