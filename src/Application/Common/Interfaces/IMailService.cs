namespace Cfo.Cats.Application.Common.Interfaces;

public interface IMailService
{
    Task SendAsync(string to, string subject, string body);
    Task SendAsync(string to, string subject, string template, object model);
}
