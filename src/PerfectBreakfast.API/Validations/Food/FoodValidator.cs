using FluentValidation;
using PerfectBreakfast.Application.Models.FoodModels.Request;

namespace PerfectBreakfast.API.Validations.Food
{
    public class FoodValidator : AbstractValidator<CreateFoodRequestModels>
    {
        public FoodValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Tên không được để trống")
                .NotEmpty().WithMessage("Tên không được để trống");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Giá không được để trống")
                .GreaterThan(0).WithMessage("Giá tiền phải lớn hơn 0");

            RuleFor(x => x.Image)
                .NotNull().WithMessage("Ảnh không được để trống")
                .NotEmpty().WithMessage("Ảnh không được để trống");
            
            RuleFor(x => x.FoodStatus)
                .NotNull().WithMessage("Trạng thái không được để trống")
                .NotEmpty().WithMessage("Trạng thái không được để trống");
            
            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("Loại món không được để trống")
                .NotEmpty().WithMessage("Loại món không được để trống");
        }
    }
}
