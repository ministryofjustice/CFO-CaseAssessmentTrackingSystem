namespace Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.DTOs;

public record ContractTargetListItemDto
{
    public Guid Id { get; init; }

    public required string ContractId { get; init; }

    public required string ContractName { get; init; }

    public int Year { get; init; }

    public int Month { get; init; }

    public string MonthName => System.Globalization.CultureInfo.CurrentCulture
        .DateTimeFormat.GetMonthName(Month);

    public int Prison { get; init; }

    public int Community { get; init; }

    public int Wings { get; init; }

    public int Hubs { get; init; }

    public int PreReleaseSupport { get; init; }

    public int ThroughTheGate { get; init; }

    public int SupportWork { get; init; }

    public int HumanCitizenship { get; init; }

    public int CommunityAndSocial { get; init; }

    public int Interventions { get; init; }

    public int Employment { get; init; }

    public int TrainingAndEducation { get; init; }
}
