using Cfo.Cats.Domain.Common.Exceptions;

namespace Cfo.Cats.Application.Pipeline;

public abstract class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUserService)
{
    protected async Task<TResponse> HandleCore(TRequest request, Func<Task<TResponse>> next)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            LogError(request, ex);
            if (TryCreateFailure(ex.Errors.Select(x => x.ErrorMessage).Distinct().ToArray(), out var failure))
            {
                return failure;
            }

            throw;
        }
        catch (DbUpdateException ex)
        {
            LogError(request, ex);
            if (TryCreateFailure(GetErrors(ex), out var failure))
            {
                return failure;
            }

            throw;
        }
        catch (Exception ex) when (ex is DomainException or ServerException)
        {
            LogError(request, ex);
            if (TryCreateFailure([ex.Message], out var failure))
            {
                return failure;
            }

            throw;
        }
        catch (Exception ex)
        {
            LogError(request, ex);
            if (TryCreateFailure(["Unexpected error"], out var failure))
            {
                return failure;
            }

            throw;
        }
    }

    private void LogError(TRequest request, Exception exception)
    {
        var requestName = typeof(TRequest).Name;
        var userName = currentUserService.UserName;

        logger.LogError(
            exception,
            "{Name}: {Exception} with {@Request} by {@UserName}",
            requestName,
            exception.Message,
            request,
            userName);
    }

    private static bool TryCreateFailure(string[] errors, out TResponse response)
    {
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidResultType = typeof(Result<>).MakeGenericType(resultType);
            var invalidResultMethod = invalidResultType.GetMethod("Failure", [typeof(string[])]);
            response = (TResponse)invalidResultMethod!.Invoke(null, [errors])!;
            return true;
        }

        if (typeof(TResponse) == typeof(Result))
        {
            response = (TResponse)(object)Result.Failure(errors);
            return true;
        }

        response = default!;
        return false;
    }

    private static string[] GetErrors(DbUpdateException exception)
    {
        IList<string> errors = new List<string>();
        if (exception.InnerException is not null)
        {
            foreach (var result in exception.Entries)
            {
                errors.Add(
                    $"A DbUpdateException was caught while saving changes. Type: {result.Entity.GetType().Name} was part of the problem. ");
            }
        }

        return errors.ToArray();
    }
}

public sealed class CommandUnhandledExceptionBehaviour<TCommand, TResponse>(
    ILogger<TCommand> logger,
    ICurrentUserService currentUserService)
    : UnhandledExceptionBehaviour<TCommand, TResponse>(logger, currentUserService),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(command, () => next());
}

public sealed class QueryUnhandledExceptionBehaviour<TQuery, TResponse>(
    ILogger<TQuery> logger,
    ICurrentUserService currentUserService)
    : UnhandledExceptionBehaviour<TQuery, TResponse>(logger, currentUserService),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore(query, () => next());
}
