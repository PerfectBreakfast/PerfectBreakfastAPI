namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record UpdateMenuRequest
    {
        public string Name { get; set; }
        public List<MenuFoodRequest?> MenuFoodRequests { get; set; }
    }
}
