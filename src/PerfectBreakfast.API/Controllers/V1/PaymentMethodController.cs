using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PaymentMethodModels.Request;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.API.Controllers.V1;

[Route("api/v{version:apiVersion}/paymentMethods")]
public class PaymentMethodController : BaseController
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    /// <summary>
    /// Api ???
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var response = await _paymentMethodService.GetPaymentMethods();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api ??
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentMethod(Guid id)
    {
        var response = await _paymentMethodService.GetPaymentMethod(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="paymentMethodRequest"></param>
    /// <returns></returns>
    [HttpPost,Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> CreatePaymentMethod(PaymentMethodRequest paymentMethodRequest)
    {
        var response = await _paymentMethodService.CreatePaymentMethod(paymentMethodRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="id"></param>
    /// <param name="paymentMethodRequest"></param>
    /// <returns></returns>
    [HttpPut("update/{id}"),Authorize(Policy = ConstantRole.RequireSuperAdminRole)]
    public async Task<IActionResult> UpdatePaymentMethod(Guid id, PaymentMethodRequest paymentMethodRequest)
    {
        var response = await _paymentMethodService.UpdatePaymentMethod(id, paymentMethodRequest);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// Api for Supper Admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeletePaymentMethod(Guid id)
    {
        var response = await _paymentMethodService.DeletePaymentMethod(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// chawcs k can
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("pagination")]
    public async Task<IActionResult> GetPaymentMethodPagination(int pageIndex = 0, int pageSize = 10)
    {
        var response = await _paymentMethodService.GetPaymentMethodPaginationAsync(pageIndex, pageSize);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }

    /// <summary>
    /// api ???
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _paymentMethodService.Delete(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response.Payload);
    }
}