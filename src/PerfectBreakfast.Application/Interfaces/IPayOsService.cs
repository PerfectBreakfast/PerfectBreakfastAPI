using Net.payOS.Types;
using PerfectBreakfast.Application.Models.PaymentModels.Request;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Models.PayOSModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IPayOsService 
{
    public Task<PaymentResponse> CreatePaymentLink(Order order);
    public Task<bool> ConfirmWebhook(ConfirmWebhook body);
    public Task<Response> HandleWebhook(WebhookType type);
}
