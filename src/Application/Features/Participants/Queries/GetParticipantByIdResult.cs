using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantByIdResult
{
    [RequestAuthorize(Policy = SecurityPolicies.Internal)]
    public class Query : IAuditableRequest<Result<ParticipantDto>>
    {
        public required string ParticipantId { get; set; }
        public string Identifier() => ParticipantId;
    }

    public class Handler(IUnitOfWork unitOfWork)
        : IRequestHandler<Query, Result<ParticipantDto>>
    {
        public async Task<Result<ParticipantDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .ApplySpecification(new ParticipantByIdSpecification(request.ParticipantId))
                .Select(p => new ParticipantDto
                {
                    //Only need to return the Id for this query, but we can easily add more properties to the DTO and return them here if needed without impacting existing consumers of GetParticipantById
                    //Also removes dependency on AutoMapper for this query which is a nice to have as it is the only query in this feature that doesn't return a Result<T>
                    Id = p.Id
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (participant is null)
            {
                return Result<ParticipantDto>.Failure(
                    $"Participant {request.ParticipantId} not found");
            }

            return Result<ParticipantDto>.Success(participant);
        }
    }

#pragma warning disable CS8618 // or IDE0051 depending on tool
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
#pragma warning restore
}