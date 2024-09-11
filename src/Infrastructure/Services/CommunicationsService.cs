using Notify.Client;

namespace Cfo.Cats.Infrastructure.Services;

public class CommunicationsService(IOptions<NotifyOptions> options, ILogger<CommunicationsService> logger) : ICommunicationsService
{
    public async Task SendSmsCodeAsync(string mobileNumber, string code)
    {
        try
        {
            var client = Client();
            var response = await client.SendSmsAsync(mobileNumber: mobileNumber,
            templateId: options.Value.GetTemplate("TwoFactorCode")!.SmsTemplateId,
            personalisation: new Dictionary<string, dynamic>()
            {
                {
                    "body", 
                    $"Your two factor authentication code is {code}"
                }
            });
        }
        catch (Exception e)
        {
            logger.LogError("Failed to send SMS code. {e}", e);
        }
    }
    public async Task SendEmailCodeAsync(string email, string code)
    {
        try
        {
            var client = Client();
            var response = await client.SendEmailAsync(emailAddress: email,
            templateId: options.Value.GetTemplate("TwoFactorCode")!.EmailTemplateId,
            personalisation: new Dictionary<string, dynamic>() {
                {
                    "subject",
                    "Your two factor authentication code."
                },
                {
                    "body", $"Your two factor authentication code is {code}"
                }
            });
        }
        catch (Exception e)
        {
            logger.LogError("Failed to send Email code. {e}", e);
        }
    }
 
    public async Task SendAccountDeactivationEmail(string email)
    {
        try
        {
            var client = Client();
            var response = await client.SendEmailAsync(emailAddress: email,
            templateId: options.Value.GetTemplate("AccountDeactivationReminder")!.EmailTemplateId,
            personalisation: new Dictionary<string, dynamic>() {
                {
                    "subject",
                    "Your account will be deactivated."
                },
                {
                    "body", 
                    "Your account will be deactivated if you do not login soon"
                }
            });
        }
        catch (Exception e)
        {
            logger.LogError("Failed to send Email. {e}", e);
        }
    }

    private NotificationClient Client() => new(options.Value.ApiKey);
}

public class NotifyOptions
{
    public const string Notify = "Notify";
    public required string ApiKey { get; set; }
    public IEnumerable<Template> Templates { get; set; } = [];
    public Template? GetTemplate(string key) => Templates.FirstOrDefault(template => template.Key == key);
}

public class Template
{
    public required string Key { get; set; }
    public string? EmailTemplateId { get; set; }
    public string? SmsTemplateId { get; set; }
}