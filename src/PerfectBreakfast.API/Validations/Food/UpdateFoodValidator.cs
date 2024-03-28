using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food;

public class UpdateFoodValidator : AbstractValidator<UpdateFoodRequestModels>
{
    public UpdateFoodValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Tên không quá 100 kí tự");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0");

    }
}
