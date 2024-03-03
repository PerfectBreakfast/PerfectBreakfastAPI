using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment;

public class SupplierFoodAssignmentsRequestValidator : AbstractValidator<SupplierFoodAssignmentsRequest>
{
    public SupplierFoodAssignmentsRequestValidator()
    {
        RuleFor(p => p.DailyOrderId)
            .NotEmpty().WithMessage("Daily order cannot be empty")
            .NotNull().WithMessage("Daily order cannot be null");

        RuleFor(s => s.SupplierFoodAssignmentRequest)
            .NotEmpty().WithMessage("SupplierFoodAssignmentRequest cannot be empty")
            .NotNull().WithMessage("SupplierFoodAssignmentRequest cannot be null");

        RuleForEach(s => s.SupplierFoodAssignmentRequest)
            .SetValidator(new SupplierFoodAssignmentRequestValidator());

    }
}