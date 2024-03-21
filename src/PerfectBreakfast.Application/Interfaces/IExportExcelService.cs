using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IExportExcelService
{
    public byte[] DownloadSupplierFoodAssignmentForSupplier(SupplierFoodAssignmentForSupplier supplierFood);
    public byte[] DownloadOrderStatisticExcel(OrderStatisticResponse response);
    public byte[] DownloadSupplierFoodAssignmentForSuperAdmin(List<SupplierFoodAssignmentForSuperAdmin> supplierFoods);
    public byte[] DownloadDailyOrderStatistic(List<DailyOrderStatisticResponse> response);
}