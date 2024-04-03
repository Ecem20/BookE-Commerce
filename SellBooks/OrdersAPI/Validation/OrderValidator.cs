using OrdersAPI.Models.Dto;
using FluentValidation;
using OrdersAPI.Data;

namespace OrdersAPI.Validation
{
    public class OrderValidator : AbstractValidator<CartsDto>
    {
        public OrderValidator()
        {
            RuleFor(order => order.CartHead.Address).NotEmpty().WithMessage("Adres giriniz");
        }
    }
}