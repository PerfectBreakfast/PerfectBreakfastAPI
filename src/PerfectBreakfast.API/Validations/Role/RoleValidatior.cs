using FluentValidation;
using PerfectBreakfast.Application.Models.RoleModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Request;

namespace PerfectBreakfast.API.Validations.Role
{
    public class RoleValidatior : AbstractValidator<CreatRoleRequest>
    {
        public RoleValidatior()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name can not null or empty");
        }
    }
}
