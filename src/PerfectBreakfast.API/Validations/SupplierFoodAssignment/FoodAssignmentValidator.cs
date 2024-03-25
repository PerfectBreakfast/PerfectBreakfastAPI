using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;

namespace PerfectBreakfast.API.Validations.SupplierFoodAssignment
{
    public class FoodAssignmentValidator : AbstractValidator<FoodAssignmentRequest>
    {
        public FoodAssignmentValidator()
        {
            RuleFor(p => p.FoodId).NotEmpty().NotNull().WithMessage("Món ăn  không được để trống");
            RuleFor(p => p.AmountCooked).NotEmpty().NotNull().WithMessage("Số lượng  không được để trống");
        }
    }
}
