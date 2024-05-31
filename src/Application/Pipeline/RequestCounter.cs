namespace Cfo.Cats.Application.Pipeline;

/// <summary>
///     Static class that holds the ExecutionCount in a shared context between different
///     instances of our PerformanceBehaviour class, regardless of the type of TRequest.
///     This allows to keep track of the number of requests application-wide.
/// </summary>
public static class RequestCounter
{
    public static int ExecutionCount;
}