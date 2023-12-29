namespace PerfectBreakfast.Application.Models.MenuModels.Request
{
    public record UpdateMenuRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
