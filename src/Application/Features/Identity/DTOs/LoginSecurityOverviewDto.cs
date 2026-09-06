namespace Cfo.Cats.Application.Features.Identity.DTOs;

public class LoginSecurityOverviewDto
{
    public int LookbackHours { get; set; }

    public int TotalFailedAttempts { get; set; }

    public int UnknownUserAttempts { get; set; }

    public int DistinctOffendingIpAddresses { get; set; }

    public int AlertsRaised { get; set; }

    public List<LoginActivityCountDto> TopOffendingIpAddresses { get; set; } = [];

    public List<LoginActivityCountDto> TopTargetedUserNames { get; set; } = [];

    public List<IdentityAuditTrailDto> RecentAlerts { get; set; } = [];
}

public class LoginActivityCountDto
{
    public string? Key { get; set; }

    public int Count { get; set; }
}
