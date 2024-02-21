using FluentValidation;
using PerfectBreakfast.Application.Models.CompanyModels.Request;

namespace PerfectBreakfast.API.Validations.Company
{
    public class CompanyValidator : AbstractValidator<CompanyRequest>
    {
        public CompanyValidator()
        {
            RuleFor(p => p.Email).NotEmpty().WithMessage("Email cannot be empty")
                .NotNull().WithMessage("Email cannot be null")
                .Matches(@"^[a-z][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").WithMessage("Invalid email format");

            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty")
                .NotNull().WithMessage("Name cannot be null")
                .MaximumLength(100)
                .Matches(@"^[\p{L}\s]+$").WithMessage("Invalid Name format");

            RuleFor(p => p.Address).NotEmpty().WithMessage("Address cannot be empty")
                .NotNull().WithMessage("Address cannot be null")
                .MaximumLength(100);
            
            RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("Phone cannot be empty")
                .NotNull().WithMessage("Phone cannot be null")
                .MaximumLength(10)
                .Matches(@"^0\d{9}$").WithMessage("Invalid phone number format");
            
            RuleFor(p => p.StartWorkHour).NotEmpty().WithMessage("Start Work Hour cannot be empty")
                .NotNull().WithMessage("Work Hour cannot be null");

            RuleFor(p => p.PartnerId).NotEmpty().WithMessage("Partner Id cannot be empty")
               .NotNull().WithMessage("Partner Unit Id cannot be null");

            RuleFor(p => p.DeliveryId).NotEmpty().WithMessage("Delivery Id cannot be empty")
               .NotNull().WithMessage("Delivery Id cannot be null");
        }
    }
}
