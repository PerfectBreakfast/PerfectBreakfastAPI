using System.Security.Cryptography;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Utils;

public static class GenerateRefreshToken
{
    public static string RandomRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}