using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class UpdateComboValidator : AbstractValidator<UpdateComboRequest>
    {
        public UpdateComboValidator()
        {
            RuleFor(x => x.Name).MaximumLength(100).WithMessage("Name can not too long");
            RuleFor(x => x.Content).MaximumLength(500).WithMessage("Content can not too long");
            //RuleFor(x => x.Image).NotNull().NotEmpty().WithMessage("Image can not null or empty");
            //RuleFor(x => x.FoodId).NotNull().NotEmpty().WithMessage("Food can not null or empty");
        }
    }
}
