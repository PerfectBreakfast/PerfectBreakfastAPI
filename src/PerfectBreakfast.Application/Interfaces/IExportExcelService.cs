using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.StatisticResponse;

namespace PerfectBreakfast.Application.Interfaces;

public interface IExportExcelService
{
    public byte[] DownloadSupplierFoodAssignmentForSupplier(List<FoodAssignmentDownloadForSupplier> supplierFood);
    public byte[] DownloadSupplierFoodAssignmentForPartner(List<FoodAssignmentDownloadForSupplier> supplierFood);
    public byte[] DownloadDailyOrderStatistic(List<DailyOrderStatisticResponse> response);
    public byte[] DownloadDailyOrderStatisticForSuperAdmin(List<DailyOrderStatisticResponse> response, List<SupplierFoodAssignmentForSuperAdmin> supplierFoods);
}