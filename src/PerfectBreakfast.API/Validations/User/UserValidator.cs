using FluentValidation;
using PerfectBreakfast.Application.Models.UserModels.Request;

namespace PerfectBreakfast.API.Validations.User;

public class UserValidator : AbstractValidator<TestUser>
{
    public UserValidator()
    {
        RuleFor(p => p.Email).NotEmpty().WithMessage("Email cannot be empty");

        RuleFor(p => p.UserName).NotEmpty().WithMessage("Phone Number cannot be empty");
    }
}