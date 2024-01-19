namespace PerfectBreakfast.Application.Models.AuthModels.Request;

public record SignUpModel
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public Guid CompanyId { get; set; }
    
}