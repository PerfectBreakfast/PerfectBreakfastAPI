using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.EmailTemplateModels;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using RazorEngineCore;

namespace PerfectBreakfast.Infrastructure.MailServices;

public class MailService : IMailService
{
    private readonly AppConfiguration _appConfiguration;

    public MailService(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }

    public async Task<bool> SendAsync(MailDataViewModel mailData, CancellationToken ct)
    {
        try
        {
            // Initialize a new instance MimeMessage 
            var mail = new MimeMessage();

            // From
            mail.From.Add(new MailboxAddress(_appConfiguration.MailSetting.DisplayName, _appConfiguration.MailSetting.From));

            //Receiver
            foreach (string mailAddress in mailData.To)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            //Add content to MineMessage
            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            if (mailData.ExcelAttachmentStream != null && !string.IsNullOrEmpty(mailData.ExcelAttachmentFileName))
            {
                body.Attachments.Add(mailData.ExcelAttachmentFileName, mailData.ExcelAttachmentStream);
            }
            mail.Body = body.ToMessageBody();

            //Send Email
            using var smtp = new SmtpClient();
            if (_appConfiguration.MailSetting.UseSsl) await smtp.ConnectAsync(_appConfiguration.MailSetting.Host, _appConfiguration.MailSetting.Port, SecureSocketOptions.SslOnConnect, ct);
            if (_appConfiguration.MailSetting.UseStartTls) await smtp.ConnectAsync(_appConfiguration.MailSetting.Host, _appConfiguration.MailSetting.Port, SecureSocketOptions.StartTls, ct);

            await smtp.AuthenticateAsync(_appConfiguration.MailSetting.UserName, _appConfiguration.MailSetting.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(MailDataViewModel mailData, CancellationToken ct)
    {
        try
        {
            // Initialize a new instance of MimeMessage 
            var mail = new MimeMessage();

            // From
            mail.From.Add(new MailboxAddress(_appConfiguration.MailSetting.DisplayName, _appConfiguration.MailSetting.From));

            // Receiver
            foreach (string mailAddress in mailData.To)
            {
                mail.To.Add(MailboxAddress.Parse(mailAddress));
            }

            // Add content to MimeMessage
            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();


            // Send Email
            using var smtp = new SmtpClient();
            if (_appConfiguration.MailSetting.UseSsl) await smtp.ConnectAsync(_appConfiguration.MailSetting.Host, _appConfiguration.MailSetting.Port, SecureSocketOptions.SslOnConnect, ct);
            if (_appConfiguration.MailSetting.UseStartTls) await smtp.ConnectAsync(_appConfiguration.MailSetting.Host, _appConfiguration.MailSetting.Port, SecureSocketOptions.StartTls, ct);

            await smtp.AuthenticateAsync(_appConfiguration.MailSetting.UserName, _appConfiguration.MailSetting.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> GetEmailTemplateForgotPassword(string nameTemplate, string clientHost, string code, User user, string email)
    {
        var mailTemplate = LoadTemplate(nameTemplate);
        var emailTemplateModel = new EmailTemplateModel
        {
            FirstName = user.Name,
            LastName = null,
            Email = user.Email,
            URL = $"{clientHost}/reset-password?token={code}&email={email}"
        };
        var razorEngine = new RazorEngine();
        var modifiledMailTemplate = razorEngine.Compile(mailTemplate);
        return modifiledMailTemplate.Run(emailTemplateModel);
    }

    public async Task<string> GetEmailTemplateNotification(string nameTemplate, string clientHost, string script, List<User?>? user, List<string> emails)
    {
        var mailTemplate = LoadTemplate(nameTemplate);
        var emailTemplateModel = new EmailTemplateModel
        {
            FirstName = "",
            LastName = null, 
            Email = "",
            URL = script
        };
        var razorEngine = new RazorEngine();
        var modifiledMailTemplate = razorEngine.Compile(mailTemplate);
        return modifiledMailTemplate.Run(emailTemplateModel);
    }

    private string LoadTemplate(string nameTemplate)
    {
        var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Template", $"{nameTemplate}.cshtml");
        using var fileStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fileStream, Encoding.Default);

        var mailTemplate = sr.ReadToEnd();
        sr.Close();
        return mailTemplate;
    }
}