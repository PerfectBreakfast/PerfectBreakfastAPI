

namespace PerfectBreakfast.Application.Models.PayOSModels.PayOSRequest;

public record ReturnPayOSRequest
{
    public int Code { get; set; }
    public string Id { get; set; }
    public string Cancel { get; set; }
    public int OrderCode { get; set; }
    public string Status { get; set; }
}
