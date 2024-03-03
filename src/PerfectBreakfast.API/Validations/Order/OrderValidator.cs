using FluentValidation;
using PerfectBreakfast.Application.Models.OrderModel.Request;

namespace PerfectBreakfast.API.Validations.Order
{
    public class OrderValidator : AbstractValidator<OrderRequest>
    {
        public OrderValidator()
        {
            RuleFor(p => p.Note)
            .MaximumLength(1000).WithMessage("Note cannot exceed 1000 characters");

            RuleFor(p => p.Payment)
                .NotNull().WithMessage("Payment cannot be null")
                .NotEmpty().WithMessage("Payment cannot be empty")
                .MaximumLength(100).WithMessage("Payment cannot exceed 100 characters");
            
            RuleFor(p => p.MealId)
                .NotNull().WithMessage("MealId cannot be null")
                .NotEmpty().WithMessage("MealId cannot be empty");

            RuleFor(p => p.OrderDetails)
                .NotNull().WithMessage("OrderDetails cannot be null")
                .NotEmpty().WithMessage("OrderDetails cannot be empty");

            RuleForEach(p => p.OrderDetails)
            .SetValidator(new OrderDetailValidator());
        }
    }
}
