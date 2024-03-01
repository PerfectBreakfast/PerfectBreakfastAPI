using Net.payOS.Types;
using Net.payOS;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PaymentModels.Request;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Payments;

public class PayOsService : IPayOsService
{
    private readonly PayOS _payOs;
    private readonly AppConfiguration _appConfiguration;
    private readonly IUnitOfWork _unitOfWork;
    public PayOsService(PayOS payOs, AppConfiguration appConfiguration,IUnitOfWork unitOfWork)
    {
        _payOs = payOs;
        _appConfiguration = appConfiguration;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponse> CreatePaymentLink(Order order)
    {
        List<ItemData> items = new ();
        PaymentData paymentData = new PaymentData(order.OrderCode, (int)order.TotalPrice, "Thanh toan don hang", items, _appConfiguration.Host+_appConfiguration.PayOSSettings.CancelURL, _appConfiguration.Host+_appConfiguration.PayOSSettings.ReturnURL);

        var result = await _payOs.createPaymentLink(paymentData);
        if (result.status == "PENDING")
        {
            var paymentResponse = new PaymentResponse
            {
                PaymentUrl = result.checkoutUrl,
                QrCode = result.qrCode,
                DeepLink = null,
                IsSuccess = true
            };
            return paymentResponse;
        }
        return new PaymentResponse{IsSuccess = false};
    }

    public async Task<bool> ConfirmWebhook(ConfirmWebhook body)
    {
        try
        {   
            await _payOs.confirmWebhook(body.webhook_url);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> HandleWebhook(WebhookType type)
    {
        try
        {
            // khong check signature
            //WebhookData data =  await _payOs.verifyPaymentWebhookData(type);
            var data = type.data;
            if (data.code == "00")
            {
                Console.WriteLine(data.desc);
                var order = await _unitOfWork.OrderRepository.GetOrderByOrderCode(data.orderCode);
                if (data.amount == order.TotalPrice)
                {
                    order.OrderStatus = OrderStatus.Paid;
                    await _unitOfWork.SaveChangeAsync();
                }
                return true;
            }
            Console.WriteLine(data.desc);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
}
