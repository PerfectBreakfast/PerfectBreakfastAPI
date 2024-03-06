using FluentValidation;
using PerfectBreakfast.Application.Models.DailyOrder.Request;

namespace PerfectBreakfast.API.Validations.DailyOrder
{
    public class DailyOrderValidator : AbstractValidator<DailyOrderRequest>
    {
        public DailyOrderValidator()
        {
            RuleFor(p => p.BookingDate).NotEmpty().WithMessage("Booking date cannot be empty");

            RuleFor(p => p.CompanyId).NotEmpty().WithMessage("Company cannot be empty");
        }
    }
}
