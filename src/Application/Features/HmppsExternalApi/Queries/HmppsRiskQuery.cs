using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.Common.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Application.Features.HmppsExternalApi.Queries;

/// <summary>
/// A query for HMPPS risk data about a single external record (CRN) held against a CATS participant.
/// Implementing <see cref="IAuditableRequest{TResponse}"/> means participant access is validated and audited by the pipeline.
/// </summary>
public interface IHmppsRiskQuery<out TResponse> : IAuditableRequest<TResponse>
{
    string ParticipantId { get; }

    string Crn { get; }
}

/// <summary>
/// Shared handling for HMPPS risk queries. Guarantees that data is only ever requested for a CRN
/// that is recorded against the requested participant, so risk data cannot be shown against the wrong record.
/// </summary>
public abstract class HmppsRiskQueryHandler<TQuery, TData>(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    IConfiguration configuration,
    IServiceProvider serviceProvider) : IQueryHandler<TQuery, Result<TData>>
    where TQuery : IHmppsRiskQuery<Result<TData>>
{
    protected abstract Task<Result<TData>> FetchAsync(IHmppsExternalApiService service, string crn, CancellationToken cancellationToken);

    public async Task<Result<TData>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        if (configuration.GetFeature(FeatureSections.HmppsRisk).IsEnabledForTenant(currentUserService.TenantId) is false)
        {
            return Result<TData>.Failure("HMPPS risk information is not available.");
        }

        // The HMPPS External API client is only registered when the integration is enabled.
        var service = serviceProvider.GetService<IHmppsExternalApiService>();

        if (service is null)
        {
            return Result<TData>.Failure("The HMPPS External API integration is currently disabled.");
        }

        var crnType = ExternalIdentifierType.Crn;

        var belongsToParticipant = await unitOfWork.DbContext.Participants
            .Where(p => p.Id == request.ParticipantId)
            .SelectMany(p => p.ExternalIdentifiers)
            .AnyAsync(e => e.Type == crnType && e.Value == request.Crn, cancellationToken);

        if (belongsToParticipant is false)
        {
            return Result<TData>.Failure("The requested record is not associated with this participant.");
        }

        return await FetchAsync(service, request.Crn, cancellationToken);
    }
}

public abstract class HmppsRiskQueryValidator<TQuery> : AbstractValidator<TQuery>
    where TQuery : IHmppsRiskQuery<object>
{
    protected HmppsRiskQueryValidator()
    {
        RuleFor(q => q.ParticipantId)
            .NotNull()
            .Length(9)
            .WithMessage("Invalid Participant Id")
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

        RuleFor(q => q.Crn)
            .NotEmpty()
            .MaximumLength(16)
            .Matches(ValidationConstants.AlphaNumeric)
            .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "CRN"));
    }
}
