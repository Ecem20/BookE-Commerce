using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MVCWeb.FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service;
using MVCWeb.Service.IService;
using Newtonsoft.Json;
using System;

namespace MVCWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private IValidator<CategoriesDto> _validator;

        public CategoryController(ICategoryService categoryService, IValidator<CategoriesDto> validator)
        {
            _categoryService = categoryService;
            _validator = validator;
        }

        public async Task<IActionResult> CategoryIndex(int pageSize = 4, int pageNumber = 1)
        {
            List<CategoriesDto>? list = new();

            ResponseDto? response = await _categoryService.GetAllCategoriesAsync(pageSize, pageNumber);

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CategoriesDto>>(Convert.ToString(response.Result));
                ViewBag.TotalPages = response.TotalPages;
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(list);
        }

        public async Task<IActionResult> CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CategoryCreate(CategoriesDto model)
        {

            ValidationResult validationResult = await _validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(model);
            }

                if (ModelState.IsValid)
                {
                ResponseDto? response = await _categoryService.CreateCategoriesAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"{model.CategoryName} isimli kategori eklenmiştir.";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CategoryDelete(int categoryid)
        {
            ResponseDto? response = await _categoryService.GetCategoryByIdAsync(categoryid);

            if (response != null && response.IsSuccess)
            {
                CategoriesDto? model = JsonConvert.DeserializeObject<CategoriesDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CategoryDelete(CategoriesDto CategoriesDto)
        {
            ResponseDto? response = await _categoryService.DeleteCategoriesAsync(CategoriesDto.CategoryId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = $"{CategoriesDto.CategoryName} isimli kategori silinmiştir.";
                return RedirectToAction(nameof(CategoryIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(CategoriesDto);
        }

        public async Task<IActionResult> CategoryEdit(int categoryId)
        {
            ResponseDto? response = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (response != null && response.IsSuccess)
            {
                CategoriesDto? model = JsonConvert.DeserializeObject<CategoriesDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CategoryEdit(CategoriesDto CategoriesDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _categoryService.UpdateCategoriesAsync(CategoriesDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"Kategori güncellenmiştir.";
                    return RedirectToAction(nameof(CategoryIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(CategoriesDto);
        }
    }
}
