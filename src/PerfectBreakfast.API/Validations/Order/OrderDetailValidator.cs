using FluentValidation;
using PerfectBreakfast.Application.Models.OrderModel.Request;

namespace PerfectBreakfast.API.Validations.Order
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailRequest>
    {
        public OrderDetailValidator()
        {
            RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0")
                .NotNull().WithMessage("Số lượng không được để trống")
                .NotEmpty().WithMessage("Số lượng không được để trống");
        }
    }
}
