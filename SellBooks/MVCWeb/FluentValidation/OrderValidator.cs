using FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service.IService;

namespace MVCWeb.FluentValidation
{
    public class OrderValidator : AbstractValidator<CartsDto>
    {
        private readonly IOrderService _orderService;

        public OrderValidator(IOrderService orderService)
        {
            _orderService = orderService;
            RuleFor(order => order.CartHead.Address).NotNull().WithMessage("Adres giriniz");
        }
    }
}