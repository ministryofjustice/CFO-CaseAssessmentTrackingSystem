using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Application.Features.Transfers.Queries;

public static class GetParticipantLatestArchiveDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Query : IRequest<Result<ArchiveDetailsDto>>
    {
        public required string ParticipantId { get; set; }
    }

    public class ArchiveDetailsDto
    {
        public string? Reason { get; set; }
        public string? AdditionalInformation { get; set; }
        public DateTime ArchivedOn { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ArchiveDetailsDto>>
    {
        public async Task<Result<ArchiveDetailsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var details = await unitOfWork.DbContext.ParticipantEnrolmentHistories
                .Where(h => h.ParticipantId == request.ParticipantId
                         && h.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value
                         && h.To == null)
                .OrderByDescending(h => h.From)
                .Select(h => new ArchiveDetailsDto
                {
                    Reason = h.Reason,
                    AdditionalInformation = h.AdditionalInformation,
                    ArchivedOn = h.From
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (details is null)
            {
                return Result<ArchiveDetailsDto>.Failure("No active archive record found for this participant.");
            }

            return Result<ArchiveDetailsDto>.Success(details);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.ParticipantId).NotEmpty();
        }
    }
}
