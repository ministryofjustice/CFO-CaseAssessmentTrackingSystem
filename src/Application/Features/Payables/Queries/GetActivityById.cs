using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Queries;

public static class GetActivityById
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Activity?>
    {
        public required Guid Id { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Activity?>
    {
        public async Task<Activity?> Handle(Query request, CancellationToken cancellationToken)
        {
            var activity = await unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .SingleOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            return activity;
        }
    }
}
