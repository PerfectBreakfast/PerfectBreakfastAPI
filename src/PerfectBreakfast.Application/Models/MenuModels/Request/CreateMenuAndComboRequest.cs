using PerfectBreakfast.Application.Models.ComboModels.Request;

namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record CreateMenuAndComboRequest
    {
        public CreateMenuFoodRequest CreateMenuFoodRequest { get; set; }
        public List<CreateComboRequest> CreateComboRequests { get; set; }
    }
}
