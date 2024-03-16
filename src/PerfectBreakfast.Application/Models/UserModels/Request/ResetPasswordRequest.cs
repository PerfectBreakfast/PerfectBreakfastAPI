namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record ResetPasswordRequest
(
    string Email,
    string Token,
    string NewPassword
    );