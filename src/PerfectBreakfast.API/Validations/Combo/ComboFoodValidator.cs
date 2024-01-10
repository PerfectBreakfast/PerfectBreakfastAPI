using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class ComboFoodValidator : AbstractValidator<ComboFoodRequest>
    {
        public ComboFoodValidator()
        {
            RuleFor(x => x.FoodId).NotNull().NotEmpty().WithMessage("Food can not null or empty");
        }
    }
}
