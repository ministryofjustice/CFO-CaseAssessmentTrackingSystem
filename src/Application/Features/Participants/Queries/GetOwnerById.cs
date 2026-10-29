using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetOwnerByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ApplicationUserSummaryDto?>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<ApplicationUserSummaryDto?>>
    {
        public async Task<Result<ApplicationUserSummaryDto?>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Single query: a null row means no participant; a null Owner means unassigned.
            var row = await unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId)
                .Select(p => new
                {
                    Owner = p.Owner == null
                        ? null
                        : new ApplicationUserSummaryDto
                        {
                            Id = p.Owner.Id,
                            UserName = p.Owner.UserName!,
                            DisplayName = p.Owner.DisplayName!,
                            TenantId = p.Owner.TenantId
                        }
                })
                .SingleOrDefaultAsync(cancellationToken);

            return row is null
                ? Result<ApplicationUserSummaryDto?>.Failure("Participant not found")
                : Result<ApplicationUserSummaryDto?>.Success(row.Owner);
        }
    }
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}
