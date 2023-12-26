using Microsoft.AspNetCore.Components;

namespace PerfectBreakfast.API.Controllers.Base;

[Route("api/v{version:apiVersion}/user")]
public class AuthenticationController : BaseController
{
    
    public AuthenticationController()
    {
        
    }
}