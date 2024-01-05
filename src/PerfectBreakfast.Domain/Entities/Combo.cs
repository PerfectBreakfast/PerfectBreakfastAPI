namespace PerfectBreakfast.Domain.Entities;

public class Combo : BaseEntity
{
    public string Name { get; set; }
    public string Content { get; set; }
    
    // relationship
    public ICollection<ComboFood?> ComboFoods { get; set; }
    public ICollection<MenuFood?> MenuFoods { get; set; }
    public ICollection<OrderDetail?> OrderDetails { get; set; }
}