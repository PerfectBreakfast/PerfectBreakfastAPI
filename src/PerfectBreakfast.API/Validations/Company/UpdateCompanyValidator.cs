using FluentValidation;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Validations.Company;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyValidator()
    {
        RuleFor(p => p.Email)
            .Matches(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

        RuleFor(p => p.Name)
            .MaximumLength(250)
            .Matches(@"^[\p{L}\s]+$").WithMessage("Invalid Name format");

        RuleFor(p => p.Address)
            .MaximumLength(250);
            
        RuleFor(p => p.PhoneNumber)
            .MaximumLength(10)
            .Matches(@"^0\d{9}$").WithMessage("Invalid phone number format");
    }
}