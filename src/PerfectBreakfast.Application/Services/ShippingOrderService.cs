﻿using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.ShippingOrder.Request;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
using PerfectBreakfast.Domain.Entities;
namespace PerfectBreakfast.Application.Services;

public class ShippingOrderService : IShippingOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    

    public ShippingOrderService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        
    }
    
    public async Task<OperationResult<ShippingOrderRespone>> CreateShippingOrder(CreateShippingOrderRequest requestModel)
    {
        var result = new OperationResult<ShippingOrderRespone>();
        try
        {
            if (requestModel.DailyOrderId.HasValue)
            {
                var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync(requestModel.DailyOrderId.Value);
                if (dailyOrder == null)
                {
                    result.AddError(ErrorCode.NotFound, "The DailyOrder with the provided ID was not found.");
                    return result;
                }
            }
            if (requestModel.ShipperId.HasValue)
            {
                var shipper = await _unitOfWork.UserManager.FindByIdAsync(requestModel.ShipperId.Value.ToString());
                if (shipper == null)
                {
                    result.AddError(ErrorCode.NotFound, "The user with the provided ID was not found.");
                    return result;
                }
                if (!(await _unitOfWork.UserManager.IsInRoleAsync(shipper, "DELIVERY STAFF")))
                {
                    result.AddError(ErrorCode.NotFound, "The user is not a Delivery Staff.");
                    return result;
                }
            }
            // map model to Entity
            var shippingOrder = _mapper.Map<ShippingOrder>(requestModel);
            shippingOrder.Status = Domain.Enums.ShippingStatus.Chờ_lấy_hàng;
            // Add to DB
            var entity = await _unitOfWork.ShippingOrderRepository.AddAsync(shippingOrder);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<ShippingOrderRespone>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}
