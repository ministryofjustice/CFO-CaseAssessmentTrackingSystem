using Cfo.Cats.Application.Pipeline.ValidationSpecifications;
using Cfo.Cats.Domain.Common.Exceptions;

namespace Cfo.Cats.Application.Pipeline;

public sealed class AccessValidationBehaviour<TQuery, TResponse>
    (IEnumerable<AccessValidationSpecification> validationStrategies,
    ILogger<AccessValidationBehaviour<TQuery, TResponse>> logger)
    : IQueryPipelineBehavior<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
{

    public async Task<TResponse> Handle(TQuery query, QueryHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(query is IAuditableRequest<TResponse> auditableRequest)
        {
            var participantId = auditableRequest.Identifier();
            
            var grant = await CanAccessParticipantRecord(participantId);

            if(grant is not AccessGrant.Granted)
            {
                logger.LogWarning("Access denied in validation pipeline");
                throw new AccessDeniedException();
            }

        }

        return await next();
    }

    private async Task<AccessGrant> CanAccessParticipantRecord(string participantId)
    {
        AccessGrant grant = AccessGrant.NotGranted;

        foreach(var strategy in validationStrategies.OrderBy(s => s.Order))
        {
            grant = await strategy.IsSatisfiedBy(participantId, grant);
            if(logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("{particpantId} {strategy} - {grant}", participantId, strategy.GetType().Name, grant);    
            }
            
            
        }
        
        return grant;
    }

}