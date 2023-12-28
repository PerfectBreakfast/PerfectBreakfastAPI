using FluentValidation;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.RoleModels.Request;

namespace PerfectBreakfast.API.Validations.CategoryValidatior
{
    public class CategoryValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
        }
    }
}
