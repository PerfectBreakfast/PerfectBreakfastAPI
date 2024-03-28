using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class ComboValidator : AbstractValidator<CreateComboRequest>
    {
        public ComboValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Tên không được để trống");
            RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("Nội dung không được để trống");
            RuleFor(x => x.Image).NotNull().NotEmpty().WithMessage("Hình ảnh không được để trống");
            RuleFor(x => x.FoodId).NotNull().NotEmpty().WithMessage("Món ăn không được để trống");
        }
    }
}
