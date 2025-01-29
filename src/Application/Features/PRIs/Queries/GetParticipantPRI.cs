using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using static Cfo.Cats.Application.Features.PRIs.Commands.AddPRI;
namespace Cfo.Cats.Application.Features.PRIs.Queries;

public static class GetParticipantPRI
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<PRIDto>>
    {
        public required string ParticipantId { get; set; }
    }
    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PRIDto>>
    {
        public async Task<Result<PRIDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var pri = await unitOfWork.DbContext.PRIs
                .Include(x => x.ExpectedReleaseRegion)
                .Include(x => x.CustodyLocation)
                .Where(x => x.ParticipantId == request.ParticipantId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (pri is null)
            {
                return Result<PRIDto>.Failure(["Pri not found."]);
            }

            return mapper.Map<PRIDto>(pri);
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotEmpty()
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(ValidationConstants.AlphaNumericMessage);

        }
    }
}
