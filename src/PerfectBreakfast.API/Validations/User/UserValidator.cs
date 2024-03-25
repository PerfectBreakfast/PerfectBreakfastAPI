using FluentValidation;
using PerfectBreakfast.Application.Models.UserModels.Request;

namespace PerfectBreakfast.API.Validations.User;

public class UserValidator : AbstractValidator<CreateUserRequestModel>
{
    public UserValidator()
    {
        RuleFor(p => p.Email).NotEmpty().WithMessage("Email cannot be empty")
            .Matches(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

        RuleFor(p => p.Name).NotEmpty().WithMessage("fullname cannot be empty")
            .MaximumLength(70)
            .Matches(@"^[\p{L}\s]+$").WithMessage("Invalid Name format");
        
        RuleFor(p => p.PhoneNumber).NotEmpty().NotNull().WithMessage("Phone number not null")
            .Matches(@"^0\d{9}$").WithMessage("Invalid phone number format");
    }
}