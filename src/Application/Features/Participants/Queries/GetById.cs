using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantById
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : ICacheableRequest<ParticipantDto>
    {
        public required string Id { get; set; }
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{Id}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, ParticipantDto>
    {
        public async Task<ParticipantDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Participants.ApplySpecification(new ParticipantByIdSpecification(request.Id))
                           .ProjectTo<ParticipantDto>(mapper.ConfigurationProvider)
                           .SingleOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException($"Participant with id: [{request.Id}] not found");
            return data;
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotNull();

            RuleFor(x => x.Id)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(RegularExpressionValidation.AlphaNumeric)
                .WithMessage(string.Format(RegularExpressionValidation.AlphaNumericMessage, "Participant Id"));


        }
    }
}
