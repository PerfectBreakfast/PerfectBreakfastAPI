using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.ComboModels.Request
{
    public record UpdateComboRequest
    {
        public string? Name { get; set; }
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        public List<Guid?> FoodId { get; set; }
    }
}
