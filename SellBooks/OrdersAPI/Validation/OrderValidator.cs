using OrdersAPI.Models.Dto;
using FluentValidation;
using OrdersAPI.Data;

namespace OrdersAPI.Validation
{
    public class OrderValidator : AbstractValidator<CartsDto>
    {
        private readonly ApplicationDbContext _db;
        public OrderValidator(ApplicationDbContext db)
        {
            _db = db;
            RuleFor(order => order.CartHead.Address).NotEmpty().WithMessage("Adres giriniz");
        }
    }
}