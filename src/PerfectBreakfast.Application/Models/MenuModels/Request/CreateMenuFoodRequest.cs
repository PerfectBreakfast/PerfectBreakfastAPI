namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record CreateMenuFoodRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<MenuFoodRequest?> MenuFoodRequests { get; set; }
    }
}
