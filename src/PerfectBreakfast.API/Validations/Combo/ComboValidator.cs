using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class ComboValidator : AbstractValidator<CreateComboRequest>
    {
        public ComboValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("Content can not null or empty");
            RuleFor(x => x.Image).NotNull().NotEmpty().WithMessage("Image can not null or empty");
            RuleFor(x => x.FoodId).NotNull().NotEmpty().WithMessage("Food can not null or empty");
        }
    }
}
