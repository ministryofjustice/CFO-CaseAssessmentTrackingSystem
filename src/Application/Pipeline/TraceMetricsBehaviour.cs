using System.Diagnostics;

namespace Cfo.Cats.Application.Pipeline;

public abstract class TraceMetricsBehaviour<TRequest, TResponse>(ICurrentUserService currentUserService)
{
    protected async Task<TResponse> HandleCore(string requestKind, Func<Task<TResponse>> next)
    {
        var requestName = GetRequestName(typeof(TRequest));

        using var activity = MediatorInstrumentation.ActivitySource.StartActivity(
            $"mediator {requestName}",
            ActivityKind.Internal);

        // Traces get the high-cardinality context (per-user / per-tenant) so individual
        // requests can be inspected in the Aspire dashboard without polluting metrics.
        activity?.SetTag("mediator.request", requestName);
        activity?.SetTag("mediator.request_kind", requestKind);
        activity?.SetTag("enduser.id", currentUserService.UserId ?? "none");
        activity?.SetTag("enduser.name", currentUserService.UserName ?? "none");
        activity?.SetTag("cats.tenant_id", currentUserService.TenantId ?? "none");
        activity?.SetTag("cats.tenant_name", currentUserService.TenantName ?? "none");

        var startTimestamp = Stopwatch.GetTimestamp();
        var inFlightTags = new TagList { { "mediator.request_kind", requestKind } };
        MediatorInstrumentation.ActiveRequests.Add(1, inFlightTags);

        var status = "success";

        try
        {
            var response = await next();

            if (response is IResult { Succeeded: false })
            {
                status = "failure";
                activity?.SetStatus(ActivityStatusCode.Error);
            }

            return response;
        }
        catch (Exception ex)
        {
            status = "error";
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
        finally
        {
            var elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;

            // Metrics stay low-cardinality: request name, kind and outcome only.
            var tags = new TagList
            {
                { "mediator.request", requestName },
                { "mediator.request_kind", requestKind },
                { "mediator.status", status },
            };

            MediatorInstrumentation.RequestDuration.Record(elapsedMs, tags);
            MediatorInstrumentation.RequestCount.Add(1, tags);
            MediatorInstrumentation.ActiveRequests.Add(-1, inFlightTags);

            activity?.SetTag("mediator.status", status);
        }
    }

    // Walks the DeclaringType chain to build a dotted name (e.g. "ParticipantsWithPagination.Query")
    // for nested requests, mirroring how the type would read in code minus the namespace.
    // Types declared directly at namespace scope (no DeclaringType) just return their own name.
    private static string GetRequestName(Type type) =>
        type.DeclaringType is { } declaringType
            ? $"{GetRequestName(declaringType)}.{type.Name}"
            : type.Name;
}

public sealed class CommandTraceMetricsBehaviour<TCommand, TResponse>(ICurrentUserService currentUserService)
    : TraceMetricsBehaviour<TCommand, TResponse>(currentUserService),
        ICommandPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(
        TCommand command,
        CommandHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore("command", () => next());
}

public sealed class QueryTraceMetricsBehaviour<TQuery, TResponse>(ICurrentUserService currentUserService)
    : TraceMetricsBehaviour<TQuery, TResponse>(currentUserService),
        IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        => HandleCore("query", () => next());
}
