using System.Collections;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class Food : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; } 
    public string Image { get; set; } = string.Empty;
    public FoodStatus FoodStatus { get; set; }
    
    //relationship
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public ICollection<MenuFood?> MenuFoods { get; set; }
    public ICollection<OrderDetail?> OrderDetails { get; set; }
    public ICollection<ComboFood?> ComboFoods { get; set; }
    public ICollection<SupplierCommissionRate?> SupplierCommissionRates { get; set; }
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }
}