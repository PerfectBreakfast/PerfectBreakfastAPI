using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

namespace PerfectBreakfast.API.Validations.SupplierCommissionRate;

public class SupplierCommissionRateValidator : AbstractValidator<CreateSupplierCommissionRateRequest>
{
    public SupplierCommissionRateValidator()
    {
        RuleFor(p => p.FoodIds)
            .NotEmpty().WithMessage("FoodIds cannot be empty")
            .NotNull().WithMessage("FoodIds cannot be null");

        RuleFor(p => p.SupplierId)
            .NotEmpty().WithMessage("SupplierId cannot be empty")
            .NotNull().WithMessage("SupplierId cannot be null");

        RuleFor(p => p.CommissionRate)
            .NotEmpty().WithMessage("Commission Rate cannot be empty")
            .NotNull().WithMessage("Commission Rate cannot be null")
            .GreaterThan(0).WithMessage("Commission Rate must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Commission Rate cannot exceed 100");
    }
}