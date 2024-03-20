using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Models.Dto;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Validation
{
    public class BookValidator : AbstractValidator<ProductDto>
    {
        private readonly ApplicationDbContext _db;
        public BookValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(book => book.ProductPrice).NotEmpty().WithMessage("Fiyat giriniz");
            RuleFor(book => book.ProductTitle).NotEmpty().WithMessage("Kitap başlığı giriniz");
            RuleFor(book => book.ProductStockQuantity).NotEmpty().WithMessage("Stok miktarı giriniz");
            RuleFor(book => book.BookPublisher).NotEmpty().WithMessage("Yayınevi giriniz");
            RuleFor(book => book.BookAuthor).NotEmpty().WithMessage("Yazar giriniz");
            RuleFor(book => book.ISBNNumber).NotEmpty().WithMessage("ISBN numarası giriniz");
            RuleFor(book => book.ProductImageURL).NotEmpty().WithMessage("Resim url'si giriniz");
            RuleFor(book => book.CategoryId).NotEmpty().WithMessage("Kategori giriniz");

            RuleFor(book => book.ProductPrice).InclusiveBetween(1, 10000).WithMessage("Fiyat 1 ile 10,000 arasında olmalıdır");
            RuleFor(book => book.ProductStockQuantity).InclusiveBetween(1, 100).WithMessage("Stok miktarı 1 ile 100 arasında olmalıdır");
            RuleFor(book => book.Count).NotEmpty().WithMessage("Miktar giriniz");
            RuleFor(book => book.ISBNNumber).Must((book, isbnNumber)=> BeUniqueISBN(isbnNumber, book.ProductId)).WithMessage("Aynı ISBN numarasına sahip başka bir kitap bulunmaktadır");
        }
        private bool BeUniqueISBN(string isbnNumber, int bookId)
        {
            var existingBook = _db.products.FirstOrDefault(b => b.ISBNNumber == isbnNumber && b.ProductId != bookId);
            return existingBook == null;
        }
    }
}