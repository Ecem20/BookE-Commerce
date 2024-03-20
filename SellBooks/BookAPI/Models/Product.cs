using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Categories Categories { get; set; }

        public decimal ProductPrice { get; set; }
        public int ProductStockQuantity { get; set; }
        public string BookAuthor { get; set; }
        public string BookPublisher { get; set; }
        public string ISBNNumber { get; set; }
        public string? ProductImageURL { get; set; }
    }
}
