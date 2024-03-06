using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food
{
    public class FoodValidator : AbstractValidator<CreateFoodRequestModels>
    {
        public FoodValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Name cannot be null")
                .NotEmpty().WithMessage("Name cannot be empty");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price cannot be null")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Image cannot be null")
                .NotEmpty().WithMessage("Image cannot be empty");
        }
    }
}
