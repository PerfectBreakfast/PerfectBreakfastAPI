using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment
{
    public class FoodAssignmentValidator : AbstractValidator<FoodAssignmentRequest>
    {
        public FoodAssignmentValidator()
        {
            RuleFor(p => p.FoodId).NotEmpty().NotNull().WithMessage("FoodId  cannot be empty");
            RuleFor(p => p.AmountCooked).NotEmpty().NotNull().WithMessage("Amount Cooked  cannot be empty");
        }
    }
}
