using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.Extensions.Configuration;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.Queries;

public static class GetArnsRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IHmppsRiskQuery<Result<RisksDto>>
    {
        public required string ParticipantId { get; init; }

        public required string Crn { get; init; }

        public string Identifier() => ParticipantId;
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
        : HmppsRiskQueryHandler<Query, RisksDto>(unitOfWork, currentUserService, configuration, serviceProvider)
    {
        protected override Task<Result<RisksDto>> FetchAsync(IHmppsExternalApiService service, string crn, CancellationToken cancellationToken)
            => service.GetSeriousHarmRiskAsync(crn, cancellationToken);
    }

    public class Validator : HmppsRiskQueryValidator<Query>;
}
