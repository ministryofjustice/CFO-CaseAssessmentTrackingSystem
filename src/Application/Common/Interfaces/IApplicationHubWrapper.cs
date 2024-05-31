namespace Cfo.Cats.Application.Common.Interfaces;

public interface IApplicationHubWrapper
{
    Task JobStarted(string message);
    Task JobCompleted(string message);
}
