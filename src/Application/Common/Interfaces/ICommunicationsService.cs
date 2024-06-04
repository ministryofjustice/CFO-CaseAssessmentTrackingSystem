namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICommunicationsService
{
    Task SendSmsCodeAsync(string mobileNumber, string code);
    Task SendEmailCodeAsync(string email, string code);
}
