using PerfectBreakfast.Application.Models.MailModels;

namespace PerfectBreakfast.Application.Interfaces;

public interface IMailService
{
    public Task<bool> SendAsync(MailDataViewModel mailData, CancellationToken ct);
}