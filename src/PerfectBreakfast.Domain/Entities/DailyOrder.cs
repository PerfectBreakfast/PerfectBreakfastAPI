using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class DailyOrder : BaseEntity
{
    public decimal? TotalPrice { get; set; }
    public int? OrderQuantity { get; set; }
    public DateOnly BookingDate { get; set; }
    public DailyOrderStatus Status { get; set; }

    // relationship
    public Guid? MealSubscriptionId { get; set; }
    
    public MealSubscription? MealSubscription { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<ShippingOrder?> ShippingOrders { get; set; }
    public ICollection<Order?> Orders { get; set; }
    public ICollection<SupplierFoodAssignment?> SupplierFoodAssignments { get; set; }
    public ICollection<ActionHistory?> ActionHistories { get; set; }

}