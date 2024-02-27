using MapsterMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using System.Drawing;
using System.Text;

namespace PerfectBreakfast.Infrastructure.BackgroundJobServices
{
    public class ManagementService : IManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentTime _currentTime;


        public ManagementService(IUnitOfWork unitOfWork, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
        }
        
        // Có nghĩa là cứ khi nào hàm này được gọi nó sẽ chuyển status và create DailyOrder mới
        public async Task AutoUpdateAndCreateDailyOrder()
        {
            try
            {
                //Update daily order each day after 4PM
                var now = _currentTime.GetCurrentTime();
                
                //Kiểm tra xem hiện tại đã qua 16h (4 PM) chưa
                // if (now.Hour < 16)
                // {
                //     Console.WriteLine("Job just run after 4PM");
                //     return;
                // }
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByBookingDate(now);
                if (dailyOrders.Count > 0)
                {
                    foreach (var dailyOrderEntity in dailyOrders)
                    {
                        var orders = dailyOrderEntity.Orders;
                        orders = orders.Where(o => o.OrderStatus == OrderStatus.Paid).ToList();
                        var totalOrderPrice = orders.Sum(o => o.TotalPrice);
                        var totalOrder = orders.Count();
                        dailyOrderEntity.TotalPrice = totalOrderPrice;
                        dailyOrderEntity.OrderQuantity = totalOrder;
                        dailyOrderEntity.Status = DailyOrderStatus.Processing;
                        _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                    }
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    Console.WriteLine("Không có dailyOrders được tạo hôm nay.");
                }
                
                //Create daily order after update
                var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
                var bookingDate = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
                var today = _currentTime.GetCurrentTime();
                
                // Kiểm tra xem đã có đơn hàng nào được tạo cho ngày hiện tại chưa
                bool isDailyOrderCreated = await _unitOfWork.DailyOrderRepository.IsDailyOrderCreated(today);
                
                // Nếu đã có đơn hàng cho ngày hiện tại, thoát khỏi hàm
                if (isDailyOrderCreated)
                {
                    Console.WriteLine("Daily order is already created");
                    return;
                }
                foreach (var co in companies)
                {
                    var company = await _unitOfWork.CompanyRepository.GetCompanyById(co.Id);
                    foreach (var meal in company.MealSubscriptions)
                    {
                        var dailyOrder = new DailyOrder()
                        {
                            BookingDate = bookingDate.AddDays(2),
                            Status = DailyOrderStatus.Initial,
                            OrderQuantity = 0,
                            TotalPrice = 0,
                            MealSubscription = meal
                        };
                        await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                    }
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR");
                throw new Exception($"{e.Message}");
            }
        }
        

    }
}
