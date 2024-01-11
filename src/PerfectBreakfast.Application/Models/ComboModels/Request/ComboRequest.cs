namespace PerfectBreakfast.Application.Models.ComboModels.Request
{
    public record ComboRequest
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
    }
}
