namespace PerfectBreakfast.Application.Models.MenuModels.Response
{
    public record MenuResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<ComboAndFoodResponse?> ComboFoodResponses { get; set; }
    }
}
