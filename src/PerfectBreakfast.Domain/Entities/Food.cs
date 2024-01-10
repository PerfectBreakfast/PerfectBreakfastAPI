using System.Collections;

namespace PerfectBreakfast.Domain.Entities;

public class Food : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } 
    public string Image { get; set; } = string.Empty;
    
    //relationship
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public ICollection<MenuFood?> MenuFoods { get; set; }
    public ICollection<OrderDetail?> OrderDetails { get; set; }
    public ICollection<ComboFood?> ComboFoods { get; set; }
    public ICollection<SupplierCommissionRate?> SupplierCommissionRates { get; set; }
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }
}