namespace PerfectBreakfast.Domain.Entities;

public class MenuFood : BaseEntity
{
    public Guid MenuId { get; set; }
    public Guid? FoodId { get; set; }
    public Guid ComboId { get; set; }
    
    public Menu? Menu { get; set; }
    public Food? Food { get; set; }
    public Combo? Combo { get; set; }
}