using FluentValidation;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Validations.Company;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyValidator()
    {
        RuleFor(p => p.Email)
            .Matches(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Email không hợp lệ");

        RuleFor(p => p.Name)
            .MaximumLength(200)
            .Matches(@"^[\p{L}\s]+$").WithMessage("Tên không hợp lệ");

        RuleFor(p => p.Address)
            .MaximumLength(250);
            
        RuleFor(p => p.PhoneNumber)
            .MaximumLength(10)
            .Matches(@"^0\d{9}$").WithMessage("SĐT không hợp lệ");
    }
}