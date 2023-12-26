namespace PerfectBreakfast.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    
    public ICollection<Food?> Foods { get; set; }
}