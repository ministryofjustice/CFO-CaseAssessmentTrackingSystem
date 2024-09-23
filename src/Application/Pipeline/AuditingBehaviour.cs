namespace Cfo.Cats.Application.Pipeline;

public class AccessAuditingBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableRequest<TResponse>
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();
        try
        {
            string currentUser = currentUserService.UserId ?? Guid.Empty.ToString();
            
            ParticipantAccessAuditTrail t = ParticipantAccessAuditTrail.Create(
                typeof(TRequest).FullName!.Split('.').Last(), 
                request.Identifier(), 
                currentUser);
            
            unitOfWork.DbContext.AccessAuditTrails.Add(t);
        }
        catch
        {
            // do nothing. We do not want audit to break the system.
        }

        return response;

    }
}