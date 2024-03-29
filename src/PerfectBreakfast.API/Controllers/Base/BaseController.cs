using Microsoft.AspNetCore.Mvc;
using PerfectBreakfast.API.Contracts.Commons;
using PerfectBreakfast.Application.Commons;

namespace PerfectBreakfast.API.Controllers.Base;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HandleErrorResponse(List<Error> errors)
    {
        if (errors.Any(e => e.Code == ErrorCode.UnAuthorize))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.UnAuthorize);
            return Unauthorized(new ErrorResponse(401,"UnAuthorize",error.Message ));
        }
        if (errors.Any(e => e.Code == ErrorCode.NotFound))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.NotFound);
            return NotFound(new ErrorResponse(404,"Not Found",error.Message));
        }
        if(errors.Any(e => e.Code == ErrorCode.ServerError))
        {
            var error = errors.FirstOrDefault(e => e.Code == ErrorCode.ServerError);
            return StatusCode(500, new ErrorResponse(500,"Server Error",error.Message));
        }
        return StatusCode(400, new ErrorResponse(400,"Bad Request",errors.First().Message));
    }
}