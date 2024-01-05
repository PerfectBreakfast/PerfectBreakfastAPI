namespace PerfectBreakfast.Domain.Entities;

public class ComboFood : BaseEntity
{
    public Guid? ComboId { get; set; }
    public Guid? FoodId { get; set; }
    
    public Combo? Combo { get; set; }
    public Food? Food { get; set; }
}