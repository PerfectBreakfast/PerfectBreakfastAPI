using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record CreateMenuAndComboRequest
    {
        public string Name { get; set; }
        public List<MenuFoodRequest?> MenuFoodRequests { get; set; }
    }
}
