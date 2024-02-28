using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food;

public class UpdateFoodValidator : AbstractValidator<UpdateFoodRequestModels>
{
    public UpdateFoodValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).WithMessage("Name can not too long");
        //RuleFor(x => x.Price).NotNull().NotEmpty().WithMessage("Price can not null or empty");
    }
}
