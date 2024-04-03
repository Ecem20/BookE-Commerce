using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IBookService
    {
        Task<ResponseDto?> GetAllBooksAsync(int pageSize, int pageNumber);
        Task<ResponseDto?> GetBookByIdAsync(int id);
        Task<ResponseDto?> GetBookByISBN(string isbn,int id);
        Task<ResponseDto?> CreateBooksAsync(ProductDto productDto);
        Task<ResponseDto?> UpdateBooksAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteBooksAsync(int id);
    }
}
