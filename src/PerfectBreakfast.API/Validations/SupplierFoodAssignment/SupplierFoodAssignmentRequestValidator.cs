using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment;

public class SupplierFoodAssignmentRequestValidator : AbstractValidator<SupplierFoodAssignmentRequest>
{
    public SupplierFoodAssignmentRequestValidator()
    {
        RuleFor(p => p.SupplierId)
            .NotEmpty().WithMessage("Supplier ID cannot be empty")
            .NotNull().WithMessage("Supplier ID cannot be null");

        RuleFor(p => p.FoodId)
            .NotEmpty().WithMessage("Food ID cannot be empty")
            .NotNull().WithMessage("Food ID cannot be null");

        RuleFor(p => p.AmountCooked)
            .NotEmpty().WithMessage("Daily order amount cannot be empty")
            .NotNull().WithMessage("Daily order amount cannot be null")
            .GreaterThan(0).WithMessage("Daily order amount must be greater than 0")
            .Must(x => x % 1 == 0).WithMessage("Daily order amount must be an integer");

    }
}