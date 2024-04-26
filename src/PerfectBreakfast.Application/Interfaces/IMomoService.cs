using PerfectBreakfast.Application.Models.MomoModels.Request;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IMomoService
{
    public Task<PaymentResponse> CreateMomoPaymentLink(Order order);
    public Task<bool> HandleIpnMomo(MomoOneTimePaymentResultRequest request);
}