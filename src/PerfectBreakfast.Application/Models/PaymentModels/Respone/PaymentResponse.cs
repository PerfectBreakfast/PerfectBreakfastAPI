

namespace PerfectBreakfast.Application.Models.PaymentModels.Respone;

public class PaymentResponse
{
    public string PaymentUrl { get; set; }
    public string? QrCode { get; set; }
    public string? DeepLink { get; set; }
    public bool IsSuccess { get; set; }
}
