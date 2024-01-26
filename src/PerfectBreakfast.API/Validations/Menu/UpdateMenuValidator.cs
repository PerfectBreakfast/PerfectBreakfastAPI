using FluentValidation;
using PerfectBreakfast.Application.Models.MenuModels.Request;

namespace PerfectBreakfast.API.Validations.Menu
{
    public class UpdateMenuValidator : AbstractValidator<UpdateMenuRequest>
    {
        public UpdateMenuValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(100);
        }
    }

}
