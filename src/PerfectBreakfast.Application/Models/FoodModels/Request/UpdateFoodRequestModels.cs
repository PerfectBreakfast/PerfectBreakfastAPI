using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.FoodModels.Request
{
    public record UpdateFoodRequestModels
    {
        public string? Name { get; set; } 
        public decimal? Price { get; set; }
        public IFormFile? Image { get; set; }
        public int? FoodStatus { get; set; }

        //relationship
        //public Guid? CategoryId { get; set; }
    }
}
