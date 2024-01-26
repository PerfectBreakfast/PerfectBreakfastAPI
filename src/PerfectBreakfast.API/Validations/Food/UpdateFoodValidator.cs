using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food;

public class UpdateFoodValidator : AbstractValidator<UpdateFoodRequestModels>
{
    public UpdateFoodValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
        RuleFor(x => x.Price).NotNull().NotEmpty().WithMessage("Price can not null or empty");
        RuleFor(x => x.Image).NotNull().NotEmpty().WithMessage("Image can not null or empty");
    }
}
