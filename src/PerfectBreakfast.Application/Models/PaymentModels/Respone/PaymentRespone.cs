

namespace PerfectBreakfast.Application.Models.PaymentModels.Respone;

public class PaymentRespone
{
    public string PaymentUrl { get; set; }
    public string? QrCode { get; set; }
    public string? DeepLink { get; set; }
    public string Status { get; set; }
}
