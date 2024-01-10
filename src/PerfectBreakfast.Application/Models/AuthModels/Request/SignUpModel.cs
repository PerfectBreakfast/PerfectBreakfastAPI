namespace PerfectBreakfast.Application.Models.AuthModels.Request;

public record SignUpModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    
    
    public Guid? RoleId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    
}