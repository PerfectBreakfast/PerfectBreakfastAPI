using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

namespace PerfectBreakfast.API.Validations.SupplierCommissionRate;

public class SupplierCommissionRateValidator : AbstractValidator<CreateSupplierCommissionRateRequest>
{
    public SupplierCommissionRateValidator()
    {
        RuleFor(p => p.CommissionRate).NotEmpty().NotNull().WithMessage("Commission Rate cannot be empty");
    }
}