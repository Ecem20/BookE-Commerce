using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using System.Drawing.Printing;

namespace MVCWeb.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseService _baseService;
        public CategoryService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateCategoriesAsync(CategoriesDto CategoriesDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CategoriesDto,
                Url = SD.BookAPIBase + "/api/category",
            });
        }

        public async Task<ResponseDto?> DeleteCategoriesAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.BookAPIBase + "/api/category/" + id
            });
        }

        public async Task<ResponseDto?> GetAllCategoriesAsync(int pageSize, int pageNumber)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + $"/api/category?pageSize={pageSize}&pageNumber={pageNumber}"
            });
        }

        public async Task<ResponseDto?> GetCategoryByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + "/api/category/" + id
            });
        }

        public async Task<ResponseDto?> UpdateCategoriesAsync(CategoriesDto CategoriesDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CategoriesDto,
                Url = SD.BookAPIBase + "/api/category/Update",
            });
        }

        public async Task<ResponseDto?> GetCategoriesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + "/api/category/getcategories",
            });
        }

        public async Task<ResponseDto?> CategoryAsync(string categoryName, int categoryId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.BookAPIBase + $"/api/category/categoryname?categoryName={categoryName}&categoryId={categoryId}",
            });
        } //aynı isimde başka bir kategori var mı 
    }
}