using MVCWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using Newtonsoft.Json;
using MVCWeb.FluentValidation;
using FluentValidation.Results;
using BookAPI.Models;
using MVCWeb.Service;
using Microsoft.EntityFrameworkCore;
using FluentValidation;


namespace MVCWeb.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private IValidator<ProductDto> _validator;


        public BookController(IBookService bookService, IValidator<ProductDto> validator,ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _validator = validator;
        }

        public async Task<IActionResult> BookIndex(int pageSize = 4, int pageNumber = 1)
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _bookService.GetAllBooksAsync(pageSize, pageNumber);

            if (response != null && response.IsSuccess)
            {
                var productList = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
                foreach (var product in productList)
                {
                    var categoryResponse = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
                    if (categoryResponse.IsSuccess)
                    {
                        var category = JsonConvert.DeserializeObject<CategoriesDto>(Convert.ToString(categoryResponse.Result));
                        product.Categories = category;
                    }
                }

                list = productList;
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

        public async Task<IActionResult> BookCreate()
        {
            ResponseDto? categoriesResponse = await _categoryService.GetCategoriesAsync();
            if (categoriesResponse != null && categoriesResponse.IsSuccess)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<CategoriesDto>>(Convert.ToString(categoriesResponse.Result));
            }
            else
            {
                TempData["error"] = categoriesResponse?.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookCreate(ProductDto model)
        {

            ValidationResult result = await _validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View();
            }

            if (ModelState.IsValid)
            {
                ResponseDto? response = await _bookService.CreateBooksAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"{model.ProductTitle} isimli kitap eklenmiştir.";

                    return RedirectToAction(nameof(BookIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> BookDelete(int bookId)
        {
            ResponseDto? response = await _bookService.GetBookByIdAsync(bookId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> BookDelete(ProductDto productDto)
        {
            ResponseDto? response = await _bookService.DeleteBooksAsync(productDto.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = $"{productDto.ProductTitle} isimli kitap silinmiştir.";
                return RedirectToAction(nameof(BookIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);
        }

        public async Task<IActionResult> BookEdit(int bookId)
        {
            ResponseDto? response = await _bookService.GetBookByIdAsync(bookId);

            ResponseDto? categoriesResponse = await _categoryService.GetCategoriesAsync();
            if (categoriesResponse != null && categoriesResponse.IsSuccess)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<CategoriesDto>>(Convert.ToString(categoriesResponse.Result));
            }
            else
            {
                TempData["error"] = categoriesResponse?.Message;
            }

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> BookEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _bookService.UpdateBooksAsync(productDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = $"Kitap güncellenmiştir.";
                    return RedirectToAction(nameof(BookIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);
        }
    }
}