namespace Cfo.Cats.Application.Pipeline;

public class AccessAuditingBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var span = SentrySdk.GetSpan()?
            .StartChild("auditing", "Participant access auditing");

        var response = await next(cancellationToken);
        try
        {
            var currentUser = currentUserService.UserId ?? Guid.Empty.ToString();
            
            // Check if the participant exists before auditing access to avoid unhandled exceptions for non-existent participants as forign keys in the audit trail on particpant table
            var exists = await unitOfWork.DbContext.Participants
                .AnyAsync(p => p.Id == request.Identifier(), cancellationToken);

            if (exists)
            {
                var auditEntry = ParticipantAccessAuditTrail.Create(
                    typeof(TRequest).FullName!.Split('.').Last(),
                    request.Identifier(),
                    currentUser);
                
                unitOfWork.DbContext.AccessAuditTrails.Add(auditEntry);
            }
        }
        catch (Exception ex)
        {
            // Ensure that audit exceptions do not break normal flow
            span?.SetExtra("AuditError", ex.Message);
        }
        finally
        {
            span?.Finish();
        }

        return response;
    }
}