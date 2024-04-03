using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCWeb.FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IAutService _autService;

        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor, IOrderService orderService,IAutService autService)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
            _orderService = orderService;
            _autService = autService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartsDto cartDto)
        {
            // Model doğrulama
            OrderValidator validator = new OrderValidator(_orderService);
            var validationResult = validator.Validate(cartDto);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(await LoadCartDTOBasedOnLoggedInUser());
            }

            CartsDto cart = await LoadCartDTOBasedOnLoggedInUser();
            cart.CartHead.Address = cartDto.CartHead.Address;

            //// Kullanıcı bilgilerini al
            var userInfo = await _autService.GetUserInfo(cart.CartHead.UserId);

            var user = JsonConvert.DeserializeObject<UserDto>(userInfo.Result.ToString());

            cart.CartHead.Name = user.Name;
            cart.CartHead.SurName = user.SurName;
            cart.CartHead.PhoneNumber = user.PhoneNumber;

            var response = await _orderService.CreateOrder(cart);
            OrderHeadDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeadDto>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                // Sipariş başarıyla oluşturuldu, sipariş kimliği oluşturma.
                int orderId = orderHeaderDto.OrderHeadId;

                // Sipariş onay sayfasına yönlendir.
                return RedirectToAction(nameof(Confirmation), new { orderId = orderId });
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View();
        }

        private async Task<CartsDto> LoadCartDTOBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartsDto cart = JsonConvert.DeserializeObject<CartsDto>(Convert.ToString(response.Result));
                return cart;
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return new CartsDto();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = $"Ürününüz silinmiştir.";
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        public async Task<IActionResult> RemoveCartItem(int cartDetailId, int countToUpdate)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveCartItemAsync(cartDetailId, countToUpdate);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        public async Task<IActionResult> AddCartItem(int cartDetailId, int countToUpdate)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.AddCartItemAsync(cartDetailId, countToUpdate);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

        public async Task<IActionResult> RemoveAll(int CartHeaderId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.ClearCartAsync(CartHeaderId);
            if (response != null & response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View();
        }

    }
}