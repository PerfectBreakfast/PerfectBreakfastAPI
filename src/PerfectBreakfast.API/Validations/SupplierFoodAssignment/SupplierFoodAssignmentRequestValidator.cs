using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment;

public class SupplierFoodAssignmentRequestValidator : AbstractValidator<SupplierFoodAssignmentRequest>
{
    public SupplierFoodAssignmentRequestValidator()
    {
        RuleFor(p => p.SupplierId)
            .NotEmpty().WithMessage(" Nhà cung cấp không được để trống")
            .NotNull().WithMessage(" Nhà cung cấp không được để trống");

        RuleFor(p => p.FoodId)
            .NotEmpty().WithMessage(" Thực phẩm không được để trống")
            .NotNull().WithMessage(" Thực phẩm không được để trống");

        RuleFor(p => p.AmountCooked)
            .NotEmpty().WithMessage("Số lượng đặt hàng hàng ngày không được để trống")
            .NotNull().WithMessage("Số lượng đặt hàng hàng ngày không được để trống")
            .GreaterThan(0).WithMessage("Số lượng đặt hàng hàng ngày phải lớn hơn 0")
            .Must(x => x % 1 == 0).WithMessage("Số lượng đặt hàng hàng ngày phải là số nguyên");
    }
}