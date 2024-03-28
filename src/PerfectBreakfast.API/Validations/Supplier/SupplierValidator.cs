using FluentValidation;
using PerfectBreakfast.Application.Models.SupplierModels.Request;

namespace PerfectBreakfast.API.Validations.Supplier;

public class SupplierValidator : AbstractValidator<CreateSupplierRequestModel>
{
    public SupplierValidator()
    {
        RuleFor(x => x.Address).NotEmpty().NotNull()
            .WithMessage("Địa chỉ không được để trống");
        RuleFor(x => x.Name).NotNull().NotEmpty()
            .WithMessage("Tên không được để trống").MaximumLength(200);
    }
}