using MVCWeb.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWeb.Service.IService;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing.Printing;
using MVCWeb.FluentValidation;
using FluentValidation;

namespace MVCWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICartService _cartService;
        private readonly ICategoryService _categoryService;
        private readonly IAutService _autService;

        public HomeController(IBookService bookService, ICartService cartService, ICategoryService categoryService, IAutService autService)
        {
            _bookService = bookService;
            _cartService = cartService;
            _categoryService = categoryService;
            _autService = autService;
        }

        public async Task<IActionResult> Index(int pageSize = 4, int pageNumber = 1)
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
                        // ProductDto nesnesine ilgili kategori bilgisini ekleyin
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

        [Authorize]
        public async Task<IActionResult> BookDetails(int bookId)
        {
            ProductDto? model = new();

            ResponseDto? response = await _bookService.GetBookByIdAsync(bookId);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                ResponseDto? categoryResponse = await _categoryService.GetCategoryByIdAsync(model.CategoryId);

                if (categoryResponse != null && categoryResponse.IsSuccess)
                {
                    CategoriesDto category = JsonConvert.DeserializeObject<CategoriesDto>(Convert.ToString(categoryResponse.Result));

                    model.Categories = category;
                }
                else
                {
                    TempData["error"] = categoryResponse?.Message;
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ActionName("BookDetails")]
        public async Task<IActionResult> BookDetails(ProductDto productDto)
        {
            var bookResponse = await _bookService.GetBookByIdAsync(productDto.ProductId);
            if (bookResponse != null && bookResponse.IsSuccess)
            {
                var book = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(bookResponse.Result));
                if (book.ProductStockQuantity < productDto.Count)
                {
                    TempData["success"] = $"Satýn almak istediðiniz üründen stoðumuzda yalnýzca {book.ProductStockQuantity} adet vardýr.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
            var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);
            string userEmail = userEmailClaim?.Value;

            CartsDto cartDto = new CartsDto()
            {
                CartHead = new CartHeadDto
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value,
                    Email = userEmail
                }
            };

            CartDetailDto cartDetailsDto = new CartDetailDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            List<CartDetailDto> cartDetailsDtos = new() { cartDetailsDto };
            cartDto.CartDetail = cartDetailsDtos;

            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = $"Kitap sepetinize eklenmiþtir.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(productDto);
        }


        public async Task<IActionResult> CustomerIndex(int pageSize = 4, int pageNumber = 1)
        {
            List<UserDto>? list = new();

            ResponseDto? response = await _autService.GetCustomers(pageSize, pageNumber);

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<UserDto>>(Convert.ToString(response.Result));
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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}