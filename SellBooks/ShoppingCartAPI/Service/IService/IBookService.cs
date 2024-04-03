using ShoppingCartAPI.Models.Dto;

namespace ShoppingCartAPI.Service.IService
{
    public interface IBookService
    {
        Task<ProductDto> GetBook(int bookId);
        Task UpdateBook(ProductDto book);

    }
}
