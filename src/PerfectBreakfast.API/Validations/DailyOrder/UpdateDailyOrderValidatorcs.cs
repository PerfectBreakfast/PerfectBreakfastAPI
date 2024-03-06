using FluentValidation;
using PerfectBreakfast.Application.Models.DailyOrder.Request;

namespace PerfectBreakfast.API.Validations.DailyOrder
{
    public class UpdateDailyOrderValidatorcs : AbstractValidator<UpdateDailyOrderRequest>
    {
        public UpdateDailyOrderValidatorcs()
        {
            RuleFor(p => p.BookingDate).NotEmpty().WithMessage("Booking date cannot be empty")
                .NotNull().WithMessage("Booking date cannot be null");
            RuleFor(p => p.CompanyId).NotEmpty().WithMessage("Company cannot be empty")
                .NotNull().WithMessage("Company cannot be null");
            RuleFor(p => p.TotalPrice).NotEmpty().WithMessage("Total price date cannot be empty")
                .NotNull().WithMessage("Total price cannot be null");
            RuleFor(p => p.OrderQuantity).NotEmpty().WithMessage("Order quantity cannot be empty")
                .NotNull().WithMessage("Order quantity cannot be null");
            RuleFor(p => p.Status).NotEmpty().WithMessage("Status cannot be empty")
                .NotNull().WithMessage("Status cannot be null");

        }
    }
}
