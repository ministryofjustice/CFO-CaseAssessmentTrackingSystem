using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Documents.Queries;
public static class GetByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]

    public class Query : IRequest<Result<DocumentDto[]>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<DocumentDto[]>>
    {
        public async Task<Result<DocumentDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            string likeCriteria = string.Format(@"Files/{0}%", request.ParticipantId);
            var query = unitOfWork.DbContext.Documents
                .Where(d => EF.Functions.Like(d.URL, likeCriteria));

            var documents = await query.ProjectTo<DocumentDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken) ?? [];

            return Result<DocumentDto[]>.Success(documents);
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId.ToString())
                .NotEmpty()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

        }
    }
}

