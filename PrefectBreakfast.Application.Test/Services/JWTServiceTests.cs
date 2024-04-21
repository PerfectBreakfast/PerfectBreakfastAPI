using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Services;
using PerfectBreakfast.Domain.Entities;
using PrefectBreakfast.Domain.Test;

namespace PrefectBreakfast.Application.Test.Services;

public class JWTServiceTests : SetupTest
{
    private readonly IJwtService _jwtService;
    private readonly AppConfiguration _appConfiguration ;

    public JWTServiceTests()
    {
        _jwtService = new JWTService(_appConfiguration, _unitOfWorkMock.Object);
    }

    
}
