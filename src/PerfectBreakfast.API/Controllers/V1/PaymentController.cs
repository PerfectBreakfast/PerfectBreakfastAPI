
using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Controllers.Base;
using PerfectBreakfast.Application.Models.PayOSModels.PayOSRequest;

namespace PerfectBreakfast.API.Controllers.V1
{
    [Route("api/v{version:apiVersion}/payments")]
    public class PaymentController : BaseController
    {
        
        [HttpGet("returnurl-payos")]
        public string ReturnPayOSUrl(ReturnPayOSRequest returnPayOSRequest)
        {
            return returnPayOSRequest.Cancel.ToString();
        }

        [HttpGet("cancelurl-payos")]
        public string CancelPayOSUrl(ReturnPayOSRequest returnPayOSRequest)
        {
            return returnPayOSRequest.Cancel.ToString();
        }
    }
}
