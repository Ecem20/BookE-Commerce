﻿namespace ShoppingCartAPI.Models.Dto
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string ISBNNo { get; set; }
        public string? ImageURL { get; set; }

    }
}