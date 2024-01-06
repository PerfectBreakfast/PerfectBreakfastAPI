using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class SupplierFoodAssignment : BaseEntity
{
    public DateOnly DateCooked { get; set; }
    public SupplierFoodAssignmentStatus Status { get; set; }
    public int AmountCooked { get; set; }
    public decimal? ReceivedAmount { get; set; }
    
    // relationship
    public Guid? FoodId { get; set; }
    public Guid? SupplierCommissionRateId { get; set; }
    public Guid? DailyOrderId { get; set; }
    
    public Food? Food { get; set; }
    public SupplierCommissionRate? SupplierCommissionRate { get; set; }
    public DailyOrder? DailyOrder { get; set; }
}