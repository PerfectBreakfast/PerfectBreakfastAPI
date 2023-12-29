using FluentValidation;
using PerfectBreakfast.Application.Models.MenuModels.Request;

namespace PerfectBreakfast.API.Validations.Menu
{
    public class MenuValidator : AbstractValidator<MenuRequest>
    {
        public MenuValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(100);
        }
    }
}
