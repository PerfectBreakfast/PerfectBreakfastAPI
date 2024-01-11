namespace PerfectBreakfast.Application.Models.ComboModels.Request
{
    public record CreateComboRequest
    {
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public List<ComboFoodRequest?> ComboFoodRequests { get; set; }
    }
}