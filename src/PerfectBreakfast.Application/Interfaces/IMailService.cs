using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IMailService
{
    public Task<bool> SendAsync(MailDataViewModel mailData, CancellationToken ct);
    public Task<bool> SendEmailAsync(MailDataViewModel mailData, CancellationToken ct);
    Task<string> GetEmailTemplateForgotPassword(string nameTemplate,string clientHost,string code,User user, string email);
    Task<string> GetEmailTemplateNotification(string nameTemplate,string clientHost,string script, List<User?>? user, List<string> emails);
}