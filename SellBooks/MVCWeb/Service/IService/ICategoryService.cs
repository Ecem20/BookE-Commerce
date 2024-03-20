using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface ICategoryService
    {
        Task<ResponseDto?> GetAllCategoriesAsync(int pageSize, int pageNumber);
        Task<ResponseDto?> GetCategoryByIdAsync(int id);
        Task<ResponseDto?> CreateCategoriesAsync(CategoriesDto CategoriesDto);
        Task<ResponseDto?> GetCategoriesAsync();

        Task<ResponseDto?> UpdateCategoriesAsync(CategoriesDto CategoriesDto);
        Task<ResponseDto?> DeleteCategoriesAsync(int id);
    }
}
