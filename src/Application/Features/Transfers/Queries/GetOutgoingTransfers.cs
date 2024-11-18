using Cfo.Cats.Application.Common.Interfaces.Locations;
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

    class Handler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILocationService locationService,
        IMapper mapper) : IRequestHandler<Query, Result<IEnumerable<OutgoingTransferDto>>>
    {
        public async Task<Result<IEnumerable<OutgoingTransferDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<OutgoingTransferDto> transfers = [];

            var locations = locationService.GetVisibleLocations(request.TenantId ?? currentUserService.TenantId!)
                .Select(location => location.Id);

            if (locations.Any())
            {
                transfers = await unitOfWork.DbContext.ParticipantOutgoingTransferQueue
                    .Where(q => locations.Contains(q.FromLocation.Id))
                    .Where(q => q.MoveOccured > DateTime.UtcNow.AddDays(-90)) 
                    .Where(q => q.IsReplaced == false)
                    .ProjectTo<OutgoingTransferDto>(mapper.ConfigurationProvider) // ProjectToPaginatedDataAsync
                    .ToListAsync(cancellationToken);
            }

            return transfers;
        }
    }

    class Validator : AbstractValidator<Query>
    {

    }

}
