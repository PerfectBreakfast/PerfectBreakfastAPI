using FluentValidation;
using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.API.Validations.Combo
{
    public class ComboValidator : AbstractValidator<CreateComboRequest>
    {
        public ComboValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
        }
    }
}
