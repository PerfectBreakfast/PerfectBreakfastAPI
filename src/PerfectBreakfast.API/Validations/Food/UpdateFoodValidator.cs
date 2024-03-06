using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food;

public class UpdateFoodValidator : AbstractValidator<UpdateFoodRequestModels>
{
    public UpdateFoodValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot be too long");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

    }
}
