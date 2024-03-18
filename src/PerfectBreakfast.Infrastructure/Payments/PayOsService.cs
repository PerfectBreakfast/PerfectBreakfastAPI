using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using Net.payOS.Types;
using Net.payOS;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.PaymentModels.Request;
using PerfectBreakfast.Application.Models.PayOSModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Payments;

public class PayOsService : IPayOsService
{
    private readonly PayOS _payOs;
    private readonly AppConfiguration _appConfiguration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTime _currentTime;
    private readonly IDistributedCache _cache;
    public PayOsService(PayOS payOs, AppConfiguration appConfiguration,
        IUnitOfWork unitOfWork,ICurrentTime currentTime,
        IDistributedCache cache)
    {
        _payOs = payOs;
        _appConfiguration = appConfiguration;
        _unitOfWork = unitOfWork;
        _currentTime = currentTime;
        _cache = cache;
    }

    public async Task<PaymentResponse> CreatePaymentLink(Order order)
    {
        List<ItemData> items = new ();
        int expiredAt = (int)(DateTime.UtcNow.AddMinutes(15).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;  // set thời gian expiredAt là 20p 
        PaymentData paymentData = new PaymentData(order.OrderCode, 
            (int)order.TotalPrice, 
            $"{_currentTime.GetCurrentTime().ToString("dd-MM-yyyy")} Thanh toan", 
            items, 
            _appConfiguration.Host+_appConfiguration.PayOSSettings.CancelURL,
            _appConfiguration.Host+_appConfiguration.PayOSSettings.ReturnURL,
            null,
            null,
            null,
            null,
            null,
            expiredAt);

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

    public async Task<Response> HandleWebhook(WebhookType type)
    {
        try
        {
            // check signature
            WebhookData data =  _payOs.verifyPaymentWebhookData(type);
            
            if (data.code == "00")
            {
                var order = await _unitOfWork.OrderRepository.GetOrderByOrderCode(data.orderCode);
                if (order.OrderStatus != OrderStatus.Pending)
                {
                    return new Response(0, "Ok", null);
                }
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetById(order.DailyOrderId!.Value);
                if (data.amount != order.TotalPrice) return new Response(0, "Ok", null);
                order.OrderStatus = OrderStatus.Paid;
                dailyOrder.TotalPrice += order.TotalPrice;
                dailyOrder.OrderQuantity++;
                _unitOfWork.DailyOrderRepository.Update(dailyOrder);
                await _unitOfWork.SaveChangeAsync();
                    
                // thực hiện xóa cache payment link đi 
                await _cache.RemoveAsync($"order-{order.Id}");
                return new Response(0, "Ok", null);
            }
            return new Response(0, "Ok", null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Response(-1, "fail", null);
        }
    }
}
