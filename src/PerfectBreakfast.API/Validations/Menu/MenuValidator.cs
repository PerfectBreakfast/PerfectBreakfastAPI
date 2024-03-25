using FluentValidation;
using PerfectBreakfast.Application.Models.MenuModels.Request;

namespace PerfectBreakfast.API.Validations.Menu
{
    public class MenuValidator : AbstractValidator<CreateMenuFoodRequest>
    {
        public MenuValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(100);
        }
    }
}
