using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record MenuFoodRequest
    {
        public Guid? FoodId { get; set; }
        public Guid? ComboId { get; set; }
        public CreateComboRequest? CreateComboRequest { get; set; }
    }
}
