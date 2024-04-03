using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Models.Dto
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }

        public int CategoryId { get; set; }
        public CategoriesDto? Categories { get; set; }

        public decimal ProductPrice { get; set; }
        public int? ProductStockQuantity { get; set; }
        public string BookAuthor { get; set; }
        public string BookPublisher { get; set; }
        public string ISBNNumber { get; set; }
        public int Count { get; set; } = 1;

        public string? ProductImageURL { get; set; }
    }
}
