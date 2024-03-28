using Hangfire;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;


namespace PerfectBreakfast.Infrastructure.BackgroundJobServices;

public class ManagementService : IManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentTime _currentTime;
    private readonly IMailService _mailService;

    public ManagementService(IUnitOfWork unitOfWork, ICurrentTime currentTime, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _currentTime = currentTime;
        _mailService = mailService;
    }

    // Có nghĩa là cứ khi nào hàm này được gọi nó sẽ chuyển status và create DailyOrder mới
    public async Task AutoUpdateAndCreateDailyOrder()
    {
        try
        {
            //Update daily order each day after 4PM or option time 
            var now = _currentTime.GetCurrentTime();
            
            var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByBookingDate(now); // lấy hết các daily đang trạng thái init hôm nay 
            if (dailyOrders.Count > 0)
            {
                foreach (var dailyOrderEntity in dailyOrders)
                {
                    // có đơn thì chuyển sang process , khong có thì chuyển sang NoOrder
                    dailyOrderEntity.Status = dailyOrderEntity is { OrderQuantity: > 0, TotalPrice: > 0 } ? DailyOrderStatus.Processing : 
                        DailyOrderStatus.NoOrders; 
                    _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                }
                await _unitOfWork.SaveChangeAsync();
            }
            else
            {
                Console.WriteLine("Không có dailyOrders được tạo hôm nay.");
            }
            
            // đoạn này gửi mail cho những thằng partner admin có OrderQuantity > 0
            // Chạy 1 job riêng để gửi mail 
            BackgroundJob.Enqueue<IManagementService>(d => d.AutoSendMailForAllPartnerAdminAndDeliveryAdminWhenDailyOrderProcessing(now));
            
            
            // Kiểm tra xem đã có đơn hàng nào được tạo cho ngày hiện tại chưa
            var isDailyOrderCreated = await _unitOfWork.DailyOrderRepository.IsDailyOrderCreated(_currentTime.GetCurrentTime());

            // Nếu đã có đơn hàng cho ngày hiện tại, thoát khỏi hàm
            if (isDailyOrderCreated)
            {
                Console.WriteLine("Daily order is already created");
                return;
            }
            
            
            // Tạo mới dailyOrder cho tất cả các bữa ăn của các công ty trong hệ thống
            var bookingDate = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
            
            var mealSubscriptions = await _unitOfWork.MealSubscriptionRepository.GetAllAsync();
            foreach (var mealSubscription in mealSubscriptions)
            {
                var dailyOrder = new DailyOrder()
                {
                    BookingDate = bookingDate.AddDays(2), // thời gian giao sẽ là 2 ngày sau từ khi mà cái dailyOrder này được khởi tạo
                    Status = DailyOrderStatus.Initial,
                    OrderQuantity = 0,
                    TotalPrice = 0,
                    MealSubscriptionId = mealSubscription.Id
                };
                await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
            }
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR");
            throw new Exception($"{e.Message}");
        }
    }

    public async Task AutoCancelOrderWhenOverTime(Guid orderId)
    {
        try
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order.OrderStatus == OrderStatus.Pending)
            {
                order.OrderStatus = OrderStatus.Cancel;
                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveChangeAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("OrderId not found");
            throw;
        }
    }

    public async Task SendMailToSupplierWhenPartnerAssignFood(MailDataViewModel model)
    {
        try
        {
            // Gửi email và xử lý kết quả
            var sendResult = await _mailService.SendAsync(model, new CancellationToken());
            if (sendResult == false)
            {
                Console.WriteLine("Gửi mail thất bại");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CheckOrderInDailyOrderCompletedAndCompleteDailyOrderShippingOrder(Guid dailyOrderId)
    {
        try
        {
            // check xem tất cả order của cái dailyOrder này đã complete hết chưa
            var isAllOrderComplete = await _unitOfWork.OrderRepository.AreAllOrdersCompleteForDailyOrder(dailyOrderId);  
            
            // nếu order đã complete hết 
            if (isAllOrderComplete)   
            {
                // lấy ra DailyOrder để chuẩn bị update 
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetById(dailyOrderId);
                
                // lấy ra shippingOrder
                var shippingOrders = await _unitOfWork.ShippingOrderRepository
                    .GetShippingOrderByDailyOrderId(dailyOrderId);
                
                // update status dailyOrder 
                dailyOrder.Status = DailyOrderStatus.Complete;
                _unitOfWork.DailyOrderRepository.Update(dailyOrder);
                
                // update ShippingOrder 
                foreach (var shippingOrder in shippingOrders)
                {
                    shippingOrder.Status = ShippingStatus.Complete;
                }
                _unitOfWork.ShippingOrderRepository.UpdateRange(shippingOrders);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            }
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AutoSendMailForAllPartnerAdminAndDeliveryAdminWhenDailyOrderProcessing(DateTime time)
    {
        try
        {
            var emails =
                await _unitOfWork.DailyOrderRepository.FindPartnerAdminEmailsByBookingDateAndStatusProcess(time);
            
            // Tạo dữ liệu email, sử dụng token trong nội dung email
            var mailData = new MailDataViewModel(
                to: emails,
                subject: "Thông báo",
                body: $"Đơn hàng hôm nay đã được tổng hợp. Các đối tác có thể thực hiện phân chia cho nhà cung cấp"
            );
            
            // Gửi email và xử lý kết quả
            var sendResult = await _mailService.SendAsync(mailData, new CancellationToken());
            if (sendResult == false)
            {
                Console.WriteLine("Gửi mail thất bại");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}