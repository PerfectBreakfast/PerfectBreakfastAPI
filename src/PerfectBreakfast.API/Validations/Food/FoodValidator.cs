using FluentValidation;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food
{
    public class FoodValidator : AbstractValidator<CreateFoodRequestModels>
    {
        public FoodValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
        }
    }
}
