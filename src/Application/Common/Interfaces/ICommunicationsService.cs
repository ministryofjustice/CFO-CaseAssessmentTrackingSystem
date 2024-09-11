namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICommunicationsService
{
    Task SendAccountDeactivationEmail(string email);
    Task SendSmsCodeAsync(string mobileNumber, string code);
    Task SendEmailCodeAsync(string email, string code);
}
