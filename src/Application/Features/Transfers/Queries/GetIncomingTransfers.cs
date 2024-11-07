using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Transfers.Queries;

public static class GetIncomingTransfers
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<IEnumerable<IncomingTransferDto>>>
    {
        public string? TenantId { get; set; }
    }

    class Handler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILocationService locationService,
        IMapper mapper) : IRequestHandler<Query, Result<IEnumerable<IncomingTransferDto>>>
    {
        public async Task<Result<IEnumerable<IncomingTransferDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<IncomingTransferDto> transfers = [];

            var locations = locationService.GetVisibleLocations(request.TenantId ?? currentUserService.TenantId!)
                .Select(location => location.Id);

            if (locations.Any())
            {
                transfers = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
                    .Where(q => locations.Contains(q.ToLocation.Id))
                    .Where(q => q.Completed == false) // Specification?
                    .ProjectTo<IncomingTransferDto>(mapper.ConfigurationProvider) // ProjectToPaginatedDataAsync
                    .ToListAsync(cancellationToken);
            }

            return transfers;
        }
    }

    class Validator : AbstractValidator<Query>
    {

    }

}
