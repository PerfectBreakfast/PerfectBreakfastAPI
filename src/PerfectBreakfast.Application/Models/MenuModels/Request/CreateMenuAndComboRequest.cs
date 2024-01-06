namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record CreateMenuAndComboRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<MenuFoodRequests> MenuFoodRequests { get; set; }
    }
}
