using Net.payOS.Types;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IPayOsService 
{
    public Task<PaymentResponse> CreatePaymentLink(Order order);
    public Task<bool> HandleWebhook(WebhookType type);
}
