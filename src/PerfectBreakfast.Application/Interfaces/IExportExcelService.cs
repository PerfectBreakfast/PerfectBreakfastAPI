using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IExportExcelService
{
    public byte[] DownloadSupplierFoodAssignmentExcel(SupplierFoodAssignmentForSupplier supplierFood);
    public byte[] DownloadOrderStatisticExcel(OrderStatisticResponse response);
}