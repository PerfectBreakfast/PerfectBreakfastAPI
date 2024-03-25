using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierModels.Request;

namespace PerfectBreakfast.API.Validations.Supplier;

public class SupplierValidator : AbstractValidator<CreateSupplierRequestModel>
{
    public SupplierValidator()
    {
        RuleFor(x => x.Address).NotEmpty().NotNull().WithMessage("Address can not null or empty");
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty").MaximumLength(200);
    }
}