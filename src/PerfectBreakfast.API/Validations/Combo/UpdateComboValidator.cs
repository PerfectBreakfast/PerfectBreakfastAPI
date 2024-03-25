using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class UpdateComboValidator : AbstractValidator<UpdateComboRequest>
    {
        public UpdateComboValidator()
        {
            RuleFor(x => x.Name).MaximumLength(200).WithMessage("Tên không được quá 200 từ");
            RuleFor(x => x.Content).MaximumLength(500).WithMessage("Mô tả không được quá 500 từ");
            //RuleFor(x => x.Image).NotNull().NotEmpty().WithMessage("Image can not null or empty");
            //RuleFor(x => x.FoodId).NotNull().NotEmpty().WithMessage("Food can not null or empty");
        }
    }
}
