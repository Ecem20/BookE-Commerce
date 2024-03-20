using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;

namespace MVCWeb.Service
{
    public class BookService : IBookService
    {
        private readonly IBaseService _baseService;
        public BookService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateBooksAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.BookAPIBase + "/api/book",
            });
        }

        public async Task<ResponseDto?> DeleteBooksAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.BookAPIBase + "/api/book/" + id
            });
        }
        public async Task<ResponseDto?> GetAllBooksAsync(int pageSize, int pageNumber)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + $"/api/book?pageSize={pageSize}&pageNumber={pageNumber}"
            });
        }

        public async Task<ResponseDto?> GetBookByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + "/api/book/" + id
            });
        }

        public async Task<ResponseDto?> GetBookByISBN(string isbn,int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.BookAPIBase}/api/book/GetBookByISBN/{isbn}/{id}"
            });
        }

        public async Task<ResponseDto?> UpdateBooksAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.BookAPIBase + "/api/book/Update",
            });
        }
    }
}