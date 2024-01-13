using Net.payOS.Types;
using Net.payOS;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Commons;
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
        List<ItemData> items = new List<ItemData>();
        PaymentData paymentData = new PaymentData(order.OrderCode, (Int32)order.TotalPrice, "Thanh toan don hang", items, _appConfiguration.Host+_appConfiguration.PayOSSettings.CancelURL, _appConfiguration.Host+_appConfiguration.PayOSSettings.ReturnURL);

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

    public async Task<bool> HandleWebhook(WebhookType type)
    {
        WebhookData data =  _payOs.verifyPaymentWebhookData(type);
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
}
