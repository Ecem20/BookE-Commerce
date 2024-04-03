using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCWeb.Models;
using MVCWeb.Service;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MVCWeb.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        public OrderController(IOrderService orderService, IEmailService emailService)
        {
            _orderService = orderService;
            _emailService = emailService;
        }


        [Authorize]
        public IActionResult Orders(int pageSize = 4, int pageNumber = 1)
        {

            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto response = _orderService.GetOrders(userId, pageSize, pageNumber).GetAwaiter().GetResult();
            IEnumerable<OrderHeadDto> orders;
            if (response != null && response.IsSuccess)
            {
                orders = JsonConvert.DeserializeObject<List<OrderHeadDto>>(Convert.ToString(response.Result));
                ViewBag.TotalPages = response.TotalPages;
            }
            else
            {
                orders = new List<OrderHeadDto>();
            }
            return View(orders);
        }

        [Authorize]
        public IActionResult OrderCustomer(int pageSize = 4, int pageNumber = 1)
        {

            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto response = _orderService.GetOrders(userId, pageSize, pageNumber).GetAwaiter().GetResult();
            IEnumerable<OrderHeadDto> orders;
            if (response != null && response.IsSuccess)
            {
                orders = JsonConvert.DeserializeObject<List<OrderHeadDto>>(Convert.ToString(response.Result));
                ViewBag.TotalPages = response.TotalPages;
            }
            else
            {
                orders = new List<OrderHeadDto>();
            }
            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeadDto orderHeaderDto = new OrderHeadDto();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeadDto>(Convert.ToString(response.Result));
            }
            if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        public async Task<IActionResult> OrderDetailForUser(int orderId)
        {
            OrderHeadDto orderHeaderDto = new OrderHeadDto();
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeadDto>(Convert.ToString(response.Result));
            }
            if (!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        [HttpPost("OrderApproved")]
        public async Task<IActionResult> OrderApproved(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Approved);

            var response1 = await _orderService.GetOrder(orderId);

            var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeadDto>(Convert.ToString(response1.Result));
            var userEmail = orderHeaderDto.Email;

            var emailDto = new EmailDto
            {
                ReceiverEmail = userEmail,
                Subject = "Sipariş Onayı",
                Body = $"{orderId} numaralı siparişiniz onaylandı."
            };
            ResponseDto? res = await _emailService.SendEmail(emailDto);


            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            var response1 = await _orderService.GetOrder(orderId);
            var orderHeaderDto = JsonConvert.DeserializeObject<OrderHeadDto>(Convert.ToString(response1.Result));
            var userEmail = orderHeaderDto.Email;

            var emailDto = new EmailDto
            {
                ReceiverEmail = userEmail,
                Subject = "Sipariş Reddi",
                Body = $"{orderId} numaralı siparişiniz iptal edildi."
            };
            ResponseDto? res = await _emailService.SendEmail(emailDto);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
    }
}