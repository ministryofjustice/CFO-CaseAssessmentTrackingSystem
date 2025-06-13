using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation.Providers;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Queries;

public static class GetCumulativeFigures
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<CumulativeFiguresDto>>
    {
        public required DateOnly EndDate { get; init; }
        public required string? ContractId { get; init; }
        public required UserProfile CurrentUser { get; init; }
    }

    private class Handler(ICumulativeProvider cumulativeProvider) : IRequestHandler<Query, Result<CumulativeFiguresDto>>
    {
        public async Task<Result<CumulativeFiguresDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var actuals = request.ContractId switch
            {
                null => await cumulativeProvider.GetActuals(new DateOnly(2025, 1, 1), request.EndDate, request.CurrentUser.Contracts),
                not null => await cumulativeProvider.GetActuals(new DateOnly(2025, 1, 1), request.EndDate, request.ContractId)
            };

            var target = request.ContractId switch
            {
                null => await cumulativeProvider.GetTargets(new DateOnly(2025, 1, 1), request.EndDate, request.CurrentUser.Contracts),
                not null => await cumulativeProvider.GetTargets(new DateOnly(2025, 1, 1), request.EndDate, request.ContractId)
            };

            return new CumulativeFiguresDto(actuals, target);
        }
    }

    private class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(s => s.EndDate)
                .GreaterThan(new DateOnly(2024, 12, 31))
                .WithMessage("End date cannot before 01 January 2025");

            // if the contract id has been passed, then
            // it must be on that's valid for the current user
            When(predicate: s => s.ContractId is not null, action: () => {
                RuleFor(s => s)
                    .Must(query => query.CurrentUser.Contracts.Any(c => c == query.ContractId));
            });

        }
    }
}
