using FluentValidation;
using PerfectBreakfast.Application.Models.AuthModels.Request;

namespace PerfectBreakfast.API.Validations.User;

public class SignUpValidator : AbstractValidator<SignUpModel>
{
    public SignUpValidator()
    {
        RuleFor(p => p.Email).NotEmpty().NotNull().WithMessage("Email cannot be empty");
        RuleFor(p => p.CompanyId).NotEmpty().NotNull().WithMessage("Company cannot be empty");
        RuleFor(p => p.PhoneNumber).NotEmpty().NotNull().WithMessage("Phone Number cannot be null");
        RuleFor(p => p.Password).NotEmpty().NotNull().WithMessage("Password cannot be null");
    }
}