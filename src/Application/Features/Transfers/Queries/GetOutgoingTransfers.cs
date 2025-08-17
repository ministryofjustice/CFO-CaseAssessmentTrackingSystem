using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Transfers.Queries;

public static class GetOutgoingTransfers
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<IEnumerable<OutgoingTransferDto>>>
    {
        public string? TenantId { get; set; }
    }

    private class Handler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper) : IRequestHandler<Query, Result<IEnumerable<OutgoingTransferDto>>>
    {
        public async Task<Result<IEnumerable<OutgoingTransferDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<OutgoingTransferDto> transfers = [];

            var userTenantId = request.TenantId ?? currentUserService.TenantId!;
            
                transfers = await unitOfWork.DbContext.ParticipantOutgoingTransferQueue
                    .Where(u => u.PreviousTenantId!.StartsWith(userTenantId))
                    .Where(q => q.MoveOccured > DateTime.UtcNow.AddDays(-90)) 
                    .Where(q => q.IsReplaced == false)
                    .Include(q => q.Participant)
                    .ProjectTo<OutgoingTransferDto>(mapper.ConfigurationProvider) 
                    .ToListAsync(cancellationToken);           

            return transfers;
        }
    }

    private class Validator : AbstractValidator<Query>
    {
    }
}
