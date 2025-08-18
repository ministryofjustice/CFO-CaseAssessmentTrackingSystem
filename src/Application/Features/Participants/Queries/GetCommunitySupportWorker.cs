using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetCommunitySupportWorker
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<PriCodeDto>>
    {
        public required string ParticipantId { get; set; }
        public required int Code { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PriCodeDto>>
    {
        public async Task<Result<PriCodeDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var priCode = await unitOfWork.DbContext.PriCodes
                .Include(p => p.CreatedBy)
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId && p.Code == request.Code, cancellationToken)
            ?? throw new NotFoundException($"Community Support Worker not found");
            //if (priCode is null)
            //{
            //    return Result<PriCodeDto>.Failure();
            //}
            return Result<PriCodeDto>.Success(mapper.Map<PriCodeDto>(priCode!.CreatedBy));
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
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(x => x.Code)
                .NotNull()
                .InclusiveBetween(100000, 999999)
                .WithMessage("Invalid Code");

        }
    }
}
