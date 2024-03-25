using FluentValidation;
using PerfectBreakfast.Application.Models.PartnerModels.Request;

namespace PerfectBreakfast.API.Validations.Partner;

public class UpdatePartnerValidator: AbstractValidator<UpdatePartnerRequest>
{
    public UpdatePartnerValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Tên không được để trống")
            .NotNull().WithMessage("Tên không được để trống")
            .MaximumLength(50);

        RuleFor(p => p.Address).NotEmpty().WithMessage("Địa chỉ không được để trống")
            .NotNull().WithMessage("Địa chỉ không được để trống")
            .MaximumLength(100);
        RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("SĐT không được để trống")
            .NotNull().WithMessage("SĐT không được để trống")
            .MaximumLength(10);
        RuleFor((o => o.CommissionRate)).NotEmpty()
            .WithMessage("% không được để trống")
            .NotNull().WithMessage("% không được để trống");
    }
}