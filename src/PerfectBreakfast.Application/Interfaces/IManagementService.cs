using PerfectBreakfast.Application.Models.MailModels;

namespace PerfectBreakfast.Application.Interfaces;

public interface IManagementService
{
    public Task AutoUpdateAndCreateDailyOrder();
    public Task AutoCancelOrderWhenOverTime(Guid orderId);
    public Task SendMailToSupplierWhenPartnerAssignFood(MailDataViewModel model); // hàm gửi mail cho các supplier khi partner phân món cho họ
    public Task CheckOrderInDailyOrderCompletedAndCompleteDailyOrderShippingOrder(Guid dailyOrderId);
}
