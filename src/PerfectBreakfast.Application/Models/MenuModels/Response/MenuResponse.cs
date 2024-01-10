using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;

namespace PerfectBreakfast.Application.Models.MenuModels.Response
{
    public record MenuResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<FoodResponse?> FoodResponses { get; set; }
        public List<ComboResponse?> ComboResponses { get; set; }
    }
}
