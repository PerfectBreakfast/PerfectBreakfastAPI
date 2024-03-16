﻿using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;


namespace PerfectBreakfast.Application.Interfaces;

public interface IShippingOrderService
{
    public Task<OperationResult<List<ShippingOrderDTO>>> GetAllShippingOrdersWithDetails();
    public Task<OperationResult<List<ShippingOrderResponse>>> CreateShippingOrder(CreateShippingOrderRequest requestModel);
    public Task<OperationResult<List<ShippingOrderHistoryForShipperResponse>>> GetShippingOrderByDeliveryStaff();
    public Task<OperationResult<List<TotalFoodForCompanyResponse>>> GetDailyOrderByShipper();
    public Task<OperationResult<DailyOrderResponse>> ConfirmShippingOrderByShipper(Guid dailyOrderId);
    public Task<OperationResult<List<TotalFoodForCompanyResponse>>> GetHistoryDailyOrderByShipper();

}
