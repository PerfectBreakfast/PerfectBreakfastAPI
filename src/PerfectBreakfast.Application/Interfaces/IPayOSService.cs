using PerfectBreakfast.Application.Models.PaymentModels.Respone;
namespace PerfectBreakfast.Application.Interfaces;

public interface IPayOSService 
{
    public Task<PaymentRespone> CreatecreatePaymentLink(int orderCode, decimal totalPrice);
}
