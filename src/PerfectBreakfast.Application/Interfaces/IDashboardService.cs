using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IDashboardService
{
    public Task<OperationResult<List<CompanyStatistic>>> GetTop10CompanyOrder(DateOnly? fromDate, DateOnly? toDate);
    public Task<OperationResult<List<TopComboAndFoodResponse>>> GetTopComboAndFood(DateOnly? fromDate, DateOnly? toDate);
    public Task<OperationResult<OrderStatisticResponse>> OrderStatistic(DateOnly? fromDate, DateOnly? toDate);
    public Task<OperationResult<List<OrderByDateStatistic>>> OrderAmountByDateStatistic(DateOnly? fromDate, DateOnly? toDate);
}