using OrdersAPI.Models.Dto;

namespace OrdersAPI.Service.IService
{
    public interface IBookService
    {
        Task<ProductDto> GetBook(int bookId);
        Task UpdateBook(ProductDto book);
    }
}
