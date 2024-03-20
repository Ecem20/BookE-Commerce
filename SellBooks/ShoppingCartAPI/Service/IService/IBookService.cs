using ShoppingCartAPI.Models.Dto;

namespace ShoppingCartAPI.Service.IService
{
    public interface IBookService
    {
        Task<IEnumerable<ProductDto>> GetBooks();
        Task<ProductDto> GetBook(int bookId);
        Task UpdateBook(ProductDto book);

    }
}
