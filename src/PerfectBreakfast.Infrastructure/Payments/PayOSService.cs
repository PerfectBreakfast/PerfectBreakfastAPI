

using Net.payOS.Types;
using Net.payOS;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Commons;

namespace PerfectBreakfast.Infrastructure.Payments;

public class PayOSService : IPayOSService
{
    private readonly PayOS _payOS;
    private readonly AppConfiguration _appConfiguration;
    public PayOSService(PayOS payOS, AppConfiguration appConfiguration)
    {
        _payOS = payOS;
        _appConfiguration = appConfiguration;
    }

    public async Task<PaymentRespone> CreatecreatePaymentLink(int orderCode, decimal totalPrice)
    {
        ItemData item = new ItemData("Mì tôm hảo hảo ly", 1, 1000);
        List<ItemData> items = new List<ItemData>();
        PaymentData paymentData = new PaymentData(orderCode, (Int32)totalPrice, $"Thanh toan don hang co ma la: {orderCode}", items, _appConfiguration.Host+_appConfiguration.PayOSSettings.CancelURL, _appConfiguration.Host+_appConfiguration.PayOSSettings.ReturnURL);

        var u = await _payOS.createPaymentLink(paymentData);
        var paymentRespone = new PaymentRespone
        {
            PaymentUrl = u.checkoutUrl,
            QrCode = u.qrCode,
            DeepLink = null,
            Status = u.status
        };
        return paymentRespone;
    }
}
