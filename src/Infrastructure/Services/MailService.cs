namespace Cfo.Cats.Infrastructure.Services;

public class MailService : IMailService
{
    public Task SendAsync(string to, string subject, string body)
    {
        throw new NotImplementedException();
    }

    public Task SendAsync(string to, string subject, string template, object model)
    {
        throw new NotImplementedException();
    }
}
