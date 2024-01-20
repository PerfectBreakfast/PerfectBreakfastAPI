using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record CreateUserRequestModel
{
    public string Name { get; set; } 
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
    
    public Guid? DeliveryUnitId { get; set; }
    public Guid? ManagementUnitId { get; set; }
    public Guid? SupplierId { get; set; }
    
    public required string RoleName { get; set; }
}