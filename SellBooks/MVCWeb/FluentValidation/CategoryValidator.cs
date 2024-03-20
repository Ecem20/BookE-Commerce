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
        }
    }
}