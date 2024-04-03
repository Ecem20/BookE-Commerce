using FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service.IService;

namespace MVCWeb.FluentValidation
{
    public class CategoryValidator : AbstractValidator<CategoriesDto>
    {
        private readonly ICategoryService _categoryService;

        public CategoryValidator(ICategoryService categoryService)
        {
            _categoryService = categoryService;

            RuleFor(category => category.CategoryName).NotEmpty().WithMessage("Kategori giriniz");
            RuleFor(category => category.CategoryName).Must((category, name) => BeUniqueCategoryName(name, category.CategoryId)).WithMessage("Bu isimde bir kategori zaten mevcut");
        }
        public bool BeUniqueCategoryName(string categoryName, int categoryId)
        {
            var existingCategory = _categoryService.CategoryAsync(categoryName, categoryId).Result;
            if(existingCategory.IsSuccess == false){
                return existingCategory == null;
            }
            return true;
        }
    }
}