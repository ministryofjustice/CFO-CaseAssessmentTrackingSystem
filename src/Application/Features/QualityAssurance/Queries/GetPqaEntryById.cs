using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Enrolments.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetPqaEntryById
{
    [RequestAuthorize(Policy = PolicyNames.CanSubmitToQA)]
    public class Query : IRequest<Result<EnrolmentQueueEntryDto>>
    {
        public Guid Id { get; set; }
        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<EnrolmentQueueEntryDto>>
    {
        public async Task<Result<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Where(a => a.Id == request.Id && request.CurrentUser!.TenantId!.StartsWith(a.TenantId))
                .ProjectTo<EnrolmentQueueEntryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entry == null)
            {
                //question: do we return a specific error if the entry exists but is at a different tenant?
                return Result<EnrolmentQueueEntryDto>.Failure("Not found");
            }

            return entry;

        }
    }
}
