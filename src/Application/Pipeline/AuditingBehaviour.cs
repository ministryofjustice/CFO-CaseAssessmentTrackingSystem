namespace Cfo.Cats.Application.Pipeline;

public sealed class AccessAuditingBehaviour<TQuery, TResponse>(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IQueryPipelineBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(
        TQuery query,
        QueryHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("auditing", "Participant access auditing");

        var response = await next();
        try
        {
            if (query is not IAuditableRequest<TResponse> auditableRequest)
            {
                return response;
            }

            var currentUser = currentUserService.UserId ?? Guid.Empty.ToString();
            var participantId = auditableRequest.Identifier();

            // Check the participant exists before writing an audit trail with a participant foreign key.
            var exists = await unitOfWork.DbContext.Participants
                .AnyAsync(p => p.Id == participantId, cancellationToken);

            if (exists)
            {
                var auditEntry = ParticipantAccessAuditTrail.Create(
                    typeof(TQuery).FullName!.Split('.').Last(),
                    participantId,
                    currentUser);

                unitOfWork.DbContext.AccessAuditTrails.Add(auditEntry);
            }
        }
        catch (Exception ex)
        {
            span?.SetExtra("AuditError", ex.Message);
        }
        finally
        {
            span?.Finish();
        }

        return response;
    }
}
