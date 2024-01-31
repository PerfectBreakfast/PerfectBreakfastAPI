using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Domain.Entities;

public class DailyOrder : BaseEntity
{
    public decimal? TotalPrice { get; set; }
    public int? OrderQuantity { get; set; }
    public DateOnly BookingDate { get; set; }
    public DailyOrderStatus Status { get; set; }

    // relationship
    public Guid? CompanyId { get; set; }
    public Guid? AdminId { get; set; }

    public Company? Company { get; set; }
    public User? Admin { get; set; }

    public ICollection<OrderHistory?> OrderHistories { get; set; }
    public ICollection<PartnerPayment?> PartnerPayments { get; set; }
    public ICollection<ShippingOrder?> ShippingOrders { get; set; }
    public ICollection<Order?> Orders { get; set; }

}