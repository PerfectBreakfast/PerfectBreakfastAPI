using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;

namespace PerfectBreakfast.API.Validations.SupplierCommissionRate;

public class SupplierCommissionRateValidator : AbstractValidator<CreateSupplierCommissionRateRequest>
{
    public SupplierCommissionRateValidator()
    {
        RuleFor(p => p.FoodIds)
            .NotEmpty().WithMessage("Món ăn không được để trống")
            .NotNull().WithMessage("Món ăn không được để trống");

        RuleFor(p => p.SupplierId)
            .NotEmpty().WithMessage("NCC không được để trống")
            .NotNull().WithMessage("NCC không được để trống");

        RuleFor(p => p.CommissionRate)
            .NotEmpty().WithMessage("% không được để trống")
            .NotNull().WithMessage("% không được để trốngl")
            .GreaterThan(0).WithMessage("% phải lớn hơn 0")
            .LessThanOrEqualTo(100).WithMessage("% không quá 100");
    }
}