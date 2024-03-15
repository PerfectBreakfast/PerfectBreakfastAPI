namespace PerfectBreakfast.Application.Commons;

public class AppConfiguration
{
    public required string DatabaseConnection { get; set; }
    public string RedisConnection { get; set; }
    public string Host { get; set; }
    public string ClientImgurId { get; set; }
    public JwtSettings JwtSettings { get; set; }
    public MailSetting MailSetting { get; set; }
    public PayOSSettings PayOSSettings { get; set; }
    public Google Google {get;set;}

}
