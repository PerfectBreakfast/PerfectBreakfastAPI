namespace PerfectBreakfast.Application.Commons;

public class AppConfiguration
{
    public required string DatabaseConnection { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public MailSetting MailSetting { get; set; }
}
