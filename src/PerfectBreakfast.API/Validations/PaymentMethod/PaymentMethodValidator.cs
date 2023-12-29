using FluentValidation;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;

namespace PerfectBreakfast.API.Validations.PaymentMethod
{
    public class PaymentMethodValidator : AbstractValidator<PaymentMethodRequest>
    {
        public PaymentMethodValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(100);
        }
    }
}
