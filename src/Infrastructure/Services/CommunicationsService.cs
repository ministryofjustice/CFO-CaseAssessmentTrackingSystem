using Notify.Client;

namespace Cfo.Cats.Infrastructure.Services;

public class CommunicationsService(IOptions<NotifyOptions> options, ILogger<CommunicationsService> logger) : ICommunicationsService
{
    public Task SendSmsCodeAsync(string mobileNumber, string code)
    {
        throw new NotImplementedException();
    }
    public async Task SendEmailCodeAsync(string email, string code)
    {
        try
        {
            var client = Client();
            var response = await client.SendEmailAsync(emailAddress: email,
            templateId: options.Value.EmailTemplate,
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
    
    private NotificationClient Client() => new(options.Value.ApiKey);
}

public class NotifyOptions
{
    public const string Notify = "Notify";
    public string ApiKey { get; set; } = default!;
    public string SmsTemplate { get; set; } = default!;
    public string EmailTemplate { get; set; } = default!;
}