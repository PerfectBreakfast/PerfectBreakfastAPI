using Microsoft.Extensions.Caching.Distributed;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Application.Models.MomoModels.Request;
using PerfectBreakfast.Application.Models.PaymentModels.Respone;
using PerfectBreakfast.Application.Models.PayOSModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.Payments;

public class MomoService : IMomoService
{
    private readonly AppConfiguration _appConfiguration;
    private readonly ICurrentTime _currentTime;
    private readonly IMailService _mailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;

    public MomoService(AppConfiguration appConfiguration,
        ICurrentTime currentTime,
        IMailService mailService,
        IUnitOfWork unitOfWork,
        IDistributedCache cache)
    {
        _appConfiguration = appConfiguration;
        _currentTime = currentTime;
        _mailService = mailService;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<PaymentResponse> CreateMomoPaymentLink(Order order)
    {
        var momoOneTimePayRequest = new MomoOneTimePaymentRequest(_appConfiguration.MomoConfig.PartnerCode,
            _currentTime.GetCurrentTime().Ticks.ToString() + order.OrderCode,
            (long)order.TotalPrice!,
            order.OrderCode.ToString(),
            "Thanh toán",
            _appConfiguration.Host+_appConfiguration.MomoConfig.ReturnUrl,
            _appConfiguration.MomoConfig.IpnUrl,
            "captureWallet",
            string.Empty);
        
        momoOneTimePayRequest.MakeSignature(_appConfiguration.MomoConfig.AccessKey,
            _appConfiguration.MomoConfig.SecretKey);
        
        return await momoOneTimePayRequest.GetLink(_appConfiguration.MomoConfig.PaymentUrl);
    }

    public async Task<bool> HandleIpnMomo(MomoOneTimePaymentResultRequest request)
    {
        try
        {
            if (request.resultCode == 0)
            {
                var order = await _unitOfWork.OrderRepository.GetOrderByOrderCode(long.Parse(request.orderId));
                if (order.OrderStatus == OrderStatus.Pending && (long)order.TotalPrice == request.amount)
                {
                    var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(order.DailyOrderId!.Value);
                    order.OrderStatus = OrderStatus.Paid;
                    dailyOrder.TotalPrice += order.TotalPrice;
                    dailyOrder.OrderQuantity++;
                    _unitOfWork.OrderRepository.Update(order);
                    _unitOfWork.DailyOrderRepository.Update(dailyOrder);
                    await _unitOfWork.SaveChangeAsync();
                    // thực hiện xóa cache payment link đi 
                    await _cache.RemoveAsync($"order-{order.Id}");
                }
            }
            return true;
        }
        catch (Exception e)
        {
            // Tạo dữ liệu email
            var mailData = new MailDataViewModel(
                to: ["hnv2311@gmail.com"],
                subject: "Thông báo lỗi ipn Momo",
                body: $"{e.Message}"
            );
            Console.WriteLine(e.Message);
            var result = await _mailService.SendEmailAsync(mailData, new CancellationToken());
            return false;
        }
    }
}