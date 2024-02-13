using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment
{
    public class SupplierFoodAssignmentValidator : AbstractValidator<SupplierFoodsAssignmentRequest>
    {
        public SupplierFoodAssignmentValidator()
        {
            RuleFor(p => p.foodAssignmentRequests).NotEmpty().NotNull().WithMessage("Food  cannot be empty");
            RuleFor(p => p.SupplierId).NotEmpty().NotNull().WithMessage("Food  cannot be empty");
            RuleForEach(p => p.foodAssignmentRequests)
            .SetValidator(new FoodAssignmentValidator());
        }
    }
}
