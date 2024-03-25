using FluentValidation;
using PerfectBreakfast.Application.Models.OrderModel.Request;

namespace PerfectBreakfast.API.Validations.Order
{
    public class OrderValidator : AbstractValidator<OrderRequest>
    {
        public OrderValidator()
        {
            RuleFor(p => p.Note)
            .MaximumLength(1000).WithMessage("Không ghi chú quá 1000 kí tự");

            RuleFor(p => p.Payment)
                .NotNull().WithMessage("Thanh toán không được để trống")
                .NotEmpty().WithMessage("Thanh toán không được để trống")
                .MaximumLength(100).WithMessage("Thanh toán không quá 100 kí tự");
            
            RuleFor(p => p.MealId)
                .NotNull().WithMessage("Bữa ăn không được để trống")
                .NotEmpty().WithMessage("Bữa ăn không được để trống");

            RuleFor(p => p.OrderDetails)
                .NotNull().WithMessage("OrderDetails cannot be null")
                .NotEmpty().WithMessage("OrderDetails cannot be empty");

            RuleForEach(p => p.OrderDetails)
            .SetValidator(new OrderDetailValidator());
        }
    }
}
