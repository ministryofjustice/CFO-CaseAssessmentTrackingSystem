using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Identity.Queries.LoginSecurity;

public static class GetLoginSecurityOverview
{
    private static readonly IdentityActionType[] FailedActionTypes =
    [
        IdentityActionType.UnknownUser,
        IdentityActionType.IncorrectPasswordEntered,
        IdentityActionType.IncorrectTwoFactorCodeEntered,
        IdentityActionType.UserAccountLockedOut
    ];

    [RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsRead)]
    public class Query : IQuery<Result<LoginSecurityOverviewDto>>
    {
        public int LookbackHours { get; set; } = 24;

        public int TopCount { get; set; } = 10;
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IQueryHandler<Query, Result<LoginSecurityOverviewDto>>
    {
        public async Task<Result<LoginSecurityOverviewDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var windowStart = DateTime.Now.AddHours(-request.LookbackHours);

            var withinWindow = unitOfWork.DbContext.IdentityAuditTrails
                .Where(a => a.DateTime >= windowStart);

            var failedAttempts = withinWindow
                .Where(a => FailedActionTypes.Contains(a.ActionType));

            var totalFailedAttempts = await failedAttempts.CountAsync(cancellationToken);

            var unknownUserAttempts = await withinWindow
                .CountAsync(a => a.ActionType == IdentityActionType.UnknownUser, cancellationToken);

            var distinctOffendingIps = await failedAttempts
                .Where(a => a.IpAddress != null)
                .Select(a => a.IpAddress)
                .Distinct()
                .CountAsync(cancellationToken);

            var alertsRaised = await withinWindow
                .CountAsync(a => a.ActionType == IdentityActionType.SuspiciousActivityDetected, cancellationToken);

            var topOffendingIps = await failedAttempts
                .Where(a => a.IpAddress != null)
                .GroupBy(a => a.IpAddress)
                .Select(g => new LoginActivityCountDto { Key = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(request.TopCount)
                .ToListAsync(cancellationToken);

            var topTargetedUsers = await failedAttempts
                .Where(a => a.UserName != null)
                .GroupBy(a => a.UserName)
                .Select(g => new LoginActivityCountDto { Key = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(request.TopCount)
                .ToListAsync(cancellationToken);

            var recentAlerts = await withinWindow
                .Where(a => a.ActionType == IdentityActionType.SuspiciousActivityDetected)
                .OrderByDescending(a => a.DateTime)
                .Take(request.TopCount * 2)
                .ProjectTo<IdentityAuditTrailDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var dto = new LoginSecurityOverviewDto
            {
                LookbackHours = request.LookbackHours,
                TotalFailedAttempts = totalFailedAttempts,
                UnknownUserAttempts = unknownUserAttempts,
                DistinctOffendingIpAddresses = distinctOffendingIps,
                AlertsRaised = alertsRaised,
                TopOffendingIpAddresses = topOffendingIps,
                TopTargetedUserNames = topTargetedUsers,
                RecentAlerts = recentAlerts
            };

            return Result<LoginSecurityOverviewDto>.Success(dto);
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.LookbackHours)
                .GreaterThan(0)
                .LessThanOrEqualTo(24 * 30)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Lookback Hours"));

            RuleFor(q => q.TopCount)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Top Count"));
        }
    }
}
