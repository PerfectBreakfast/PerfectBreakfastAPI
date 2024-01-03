namespace PerfectBreakfast.Domain.Entities;

public class OrderDetail : BaseEntity
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public Guid? OrderId { get; set; }
    public Guid? FoodId { get; set; }
    public Guid? ComboId { get; set; }
    
    public Order? Order { get; set; }
    public Food? Food { get; set; }
    public Combo? Combo { get; set; }
}