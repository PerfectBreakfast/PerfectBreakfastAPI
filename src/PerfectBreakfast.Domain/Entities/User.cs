namespace PerfectBreakfast.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    
    // relationship 
    public Guid? CompanyId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? RoleId { get; set; }
    
    
    public Company? Company { get; set; }
    public DeliveryUnit? DeliveryUnit { get; set; }
    public ManagementUnit? ManagementUnit { get; set; }
    public Supplier? Supplier { get; set; }
    public Role? Role { get; set; }
    
    public ICollection<OrderHistory?> OrderHistories { get; set; }
    public ICollection<Order?> OrdersWorker { get; set; }
    public ICollection<Order?> OrdersShipper { get; set; }
    
}