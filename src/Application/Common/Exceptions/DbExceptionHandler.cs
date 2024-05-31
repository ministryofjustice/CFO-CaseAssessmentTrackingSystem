namespace Cfo.Cats.Application.Common.Exceptions;

public class DbExceptionHandler<TRequest, TResponse, TException>
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse : Result<int>
    where TException : DbUpdateException
{
    private readonly ILogger<DbExceptionHandler<TRequest, TResponse, TException>> _logger;

    public DbExceptionHandler(ILogger<DbExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        TRequest request,
        TException exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken
    )
    {
        state.SetHandled((TResponse)Result<int>.Failure(GetErrors(exception)));
        return Task.CompletedTask;
    }

    private static string[] GetErrors(DbUpdateException exception)
    {
        IList<string> errors = new List<string>();
        if (exception.InnerException is not null)
        {
            foreach (var result in exception.Entries)
            {
                errors.Add(
                    $"A DbUpdateException was caught while saving changes. Type: {result.Entity.GetType().Name} was part of the problem. "
                );
            }
        }

        return errors.ToArray();
    }
}
