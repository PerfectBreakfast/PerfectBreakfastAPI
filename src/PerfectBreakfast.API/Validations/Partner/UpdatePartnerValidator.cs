using FluentValidation;
using PerfectBreakfast.Application.Models.PartnerModels.Request;

namespace PerfectBreakfast.API.Validations.Partner;

public class UpdatePartnerValidator: AbstractValidator<UpdatePartnerRequest>
{
    public UpdatePartnerValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty")
            .NotNull().WithMessage("Name cannot be null")
            .MaximumLength(250);

        RuleFor(p => p.Address).NotEmpty().WithMessage("Address cannot be empty")
            .NotNull().WithMessage("Address cannot be null")
            .MaximumLength(250);
        RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("Phone cannot be empty")
            .NotNull().WithMessage("Phone cannot be null")
            .MaximumLength(10);
        RuleFor((o => o.CommissionRate)).NotEmpty().WithMessage("Commission Rate cannot be empty").NotNull().WithMessage("Commission Rate cannot be null");
    }
}