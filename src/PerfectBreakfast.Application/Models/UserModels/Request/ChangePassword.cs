namespace PerfectBreakfast.Application.Models.UserModels.Request;

public record ChangePassword
    (
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
        );