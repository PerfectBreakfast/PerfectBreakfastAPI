using FluentValidation;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Validations.Company
{
    public class CompanyValidator : AbstractValidator<CompanyRequest>
    {
        public CompanyValidator()
        {
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email cannot be empty");

            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(50);

            RuleFor(p => p.Address).NotEmpty().WithMessage("Address cannot be empty")
                .MaximumLength(100);
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("Phone cannot be empty")
                .MaximumLength(20);
        }
    }
}
