using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/shippingorders")]
    public class ShippingOrderController : ControllerBase
    {
        private readonly IShippingOrderService _shippingOrderService;
        public ShippingOrderController(IShippingOrderService shippingOrderService)
        {
            _shippingOrderService = shippingOrderService;
        }

    }
}
