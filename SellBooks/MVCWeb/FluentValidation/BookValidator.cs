using FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service.IService;

namespace MVCWeb.FluentValidation
{
    public class BookValidator : AbstractValidator<ProductDto>
    {
        private readonly IBookService _bookService;

        public BookValidator(IBookService bookService)
        {
            _bookService = bookService;

            RuleFor(book => book.ISBNNumber).Must((book, isbnNumber) => BeUniqueISBN(isbnNumber, book.ProductId)).WithMessage("A book with the same ISBN number already exists");
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
        }

        private bool BeUniqueISBN(string isbn, int id)
        {
            var existingBook = _bookService.GetBookByISBN(isbn, id);
            return existingBook == null;
        }
    }
}