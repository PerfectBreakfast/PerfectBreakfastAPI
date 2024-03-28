using FluentValidation;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Validations.Company
{
    public class CompanyValidator : AbstractValidator<CompanyRequest>
    {
        public CompanyValidator()
        {
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email không được để trống")
                .NotNull().WithMessage("Email không được để trống")
                .Matches(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Email sai");

            RuleFor(p => p.Name).NotEmpty().WithMessage("Tên không được để trống")
                .NotNull().WithMessage("Tên không được để trống")
                .MaximumLength(200)
                .Matches(@"^[\p{L}\s]+$").WithMessage("Tên không lệ");

            RuleFor(p => p.Address).NotEmpty().WithMessage("Địa chỉ không được để trống")
                .NotNull().WithMessage("Địa chỉ không được để trống")
                .MaximumLength(200);
            
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("SĐT không được để trống")
                .NotNull().WithMessage("SĐT không được để trống")
                .MaximumLength(10)
                .Matches(@"^0\d{9}$").WithMessage("SĐT không hợp lệ");

            RuleFor(p => p.PartnerId).NotEmpty().WithMessage("Partner Id cannot be empty")
               .NotNull().WithMessage("Partner Unit Id cannot be null");

            RuleFor(p => p.DeliveryId).NotEmpty().WithMessage("Delivery Id cannot be empty")
               .NotNull().WithMessage("Delivery Id cannot be null");
        }
    }
}
