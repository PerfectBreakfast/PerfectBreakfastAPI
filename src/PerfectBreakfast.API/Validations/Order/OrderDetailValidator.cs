using FluentValidation;
using PerfectBreakfast.Application.Models.OrderModel.Request;

namespace PerfectBreakfast.API.Validations.Order
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailRequest>
    {
        public OrderDetailValidator()
        {
            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0")
                .NotNull().WithMessage("Quantity cannot be null")
                .NotEmpty().WithMessage("Quantity cannot be empty");
        }
    }
}
