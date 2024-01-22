namespace PerfectBreakfast.Domain.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsSelected { get; set; } 
    
    public ICollection<MenuFood?> MenuFoods { get; set; }
}