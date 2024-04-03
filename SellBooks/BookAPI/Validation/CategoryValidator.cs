using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Models.Dto;
using FluentValidation;
using System.Net;

namespace BookAPI.Validation
{
    public class CategoryValidator : AbstractValidator<CategoriesDto>
    {
        private readonly ApplicationDbContext _db;
        public CategoryValidator(ApplicationDbContext db)
        {
            _db = db;
            RuleFor(category => category.CategoryName).NotEmpty().WithMessage("Kategori giriniz");
            RuleFor(category => category.CategoryName).Must((category, name) => BeUniqueCategoryName(name, category.CategoryId)).WithMessage("Bu isimde bir kategori zaten mevcut");
        }

        private bool BeUniqueCategoryName(string categoryName,int categoryId)
        {
            var existingcategory = _db.categories.FirstOrDefault(b => b.CategoryName == categoryName && b.CategoryId != categoryId);
            return existingcategory == null;
        }
    }
}