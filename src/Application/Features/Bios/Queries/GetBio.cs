using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Features.Bios.DTOs;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Bios.Queries;

public static class GetBio
{

    /// <summary>
    /// Returns a Bio for a Participant
    /// </summary>
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<Bio>>
    {
        public required string ParticipantId { get; set; }
        public Guid? BioId { get; set; }

    }

    internal class Handler : IRequestHandler<Query, Result<Bio>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public Handler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Result<Bio>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.DbContext.ParticipantBios
                .Where(p => p.ParticipantId == request.ParticipantId);

            if (request.BioId is not null)
            {
                query = query.Where(p => p.Id == request.BioId);
            }

            var bioSurvey = await query.OrderByDescending(bioSurvey => bioSurvey.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (bioSurvey is null)
            {
                return Result<Bio>.Failure(["Participant not found"]);
            }

            Bio bio = JsonConvert.DeserializeObject<Bio>(bioSurvey.BioJson,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
            return Result<Bio>.Success(bio);
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