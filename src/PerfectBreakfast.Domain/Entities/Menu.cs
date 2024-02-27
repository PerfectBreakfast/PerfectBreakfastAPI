namespace PerfectBreakfast.Domain.Entities;

public class Menu : BaseEntity
{
    public required string Name { get; set; } 
    public bool IsSelected { get; set; } 
    
    public ICollection<MenuFood?> MenuFoods { get; set; }
}