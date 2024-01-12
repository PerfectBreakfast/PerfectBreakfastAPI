using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

namespace PerfectBreakfast.API.Validations.SupplierCommissionRate;

public class SupplierCommissionRateValidator : AbstractValidator<CreateSupplierCommissionRateRequest>
{
    public SupplierCommissionRateValidator()
    {
        RuleFor(p => p.CommissionRate).NotEmpty().NotNull().WithMessage("Commission Rate cannot be empty");
        
        RuleFor(p => p.FoodId).NotEmpty().WithMessage("Food Id cannot be empty")
            .NotNull().WithMessage("Food Id cannot be null");

        RuleFor(p => p.SupplierId).NotEmpty().WithMessage("Supplier Id cannot be empty")
            .NotNull().WithMessage("Supplier Id cannot be null");
    }
}