using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.ComboModels.Response
{
    public record ComboFoodResponse
    {
        public Guid? FoodId { get; set; }
        public Food? Food { get; set; }
    }
}
