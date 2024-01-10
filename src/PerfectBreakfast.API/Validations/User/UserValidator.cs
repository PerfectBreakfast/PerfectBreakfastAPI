using FluentValidation;
using PerfectBreakfast.Application.Models.UserModels.Request;

namespace PerfectBreakfast.API.Validations.User;

public class UserValidator : AbstractValidator<CreateUserRequestModel>
{
    public UserValidator()
    {
        RuleFor(p => p.Email).NotEmpty().WithMessage("Email cannot be empty");

        RuleFor(p => p.FullName).NotEmpty().WithMessage("fullname cannot be empty")
            .MaximumLength(50);
    }
}