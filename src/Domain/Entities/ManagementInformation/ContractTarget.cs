namespace Cfo.Cats.Domain.Entities.ManagementInformation;

/// <summary>
/// Represents the monthly targets for a single contract.
/// Targets vary by contract and by month (Year + Month).
/// </summary>
public class ContractTarget
{
#pragma warning disable CS8618
    internal ContractTarget()
#pragma warning restore CS8618
    {
        // this is for EF Core
    }

    public Guid Id { get; set; } = Guid.CreateVersion7();

    public string ContractId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public int Prison { get; set; }

    public int Community { get; set; }

    public int Wings { get; set; }

    public int Hubs { get; set; }

    public int PreReleaseSupport { get; set; }

    public int ThroughTheGate { get; set; }

    public int SupportWork { get; set; }

    public int HumanCitizenship { get; set; }

    public int CommunityAndSocial { get; set; }

    public int Interventions { get; set; }

    public int Employment { get; set; }

    public int TrainingAndEducation { get; set; }
}
