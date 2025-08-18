using System.Diagnostics.CodeAnalysis;

namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record ContractTargetDto
{
    public ContractTargetDto()
    {
    }

    public static ContractTargetDto operator +(ContractTargetDto a, ContractTargetDto b)
    {
        return a with
        {
            Prison = a.Prison + b.Prison,
            Community = a.Community + b.Community,
            CommunityAndSocial = a.CommunityAndSocial + b.CommunityAndSocial,
            Employment = a.Employment + b.Employment,
            Wings = a.Wings + b.Wings,
            Hubs = a.Hubs + b.Hubs,
            PreReleaseSupport = a.PreReleaseSupport + b.PreReleaseSupport,
            ThroughTheGate = a.ThroughTheGate + b.ThroughTheGate,   
            SupportWork = a.SupportWork + b.SupportWork,
            HumanCitizenship = a.HumanCitizenship + b.HumanCitizenship,
            Interventions = a.Interventions + b.Interventions,
            TrainingAndEducation = a.TrainingAndEducation + b.TrainingAndEducation,
        };
    }

    [SetsRequiredMembers]
    public ContractTargetDto(string contract, int prison, int community, int wings, int hubs, int preReleaseSupport, int throughTheGate, int supportWork,  int humanCitizenship, int communityAndSocial, int interventions, int employment, int trainingAndEducation)
    {
        Contract = contract;
        Prison = prison;
        Community = community;
        Wings = wings;
        Hubs = hubs;
        PreReleaseSupport = preReleaseSupport;
        ThroughTheGate = throughTheGate;
        SupportWork = supportWork;
        HumanCitizenship = humanCitizenship;
        CommunityAndSocial = communityAndSocial;
        Interventions = interventions;
        Employment = employment;
        TrainingAndEducation = trainingAndEducation;
    }

    

    public required string Contract { get; init; }
    public required int Prison { get; init; }
    public required int Community { get; init; } 
    public required int Wings { get; init; }
    public required int Hubs { get; init; }
    public required int PreReleaseSupport { get; init; }
    public required int ThroughTheGate { get; init; }
    public required int SupportWork { get; init; }
    public required int HumanCitizenship { get; init; }
    public required int CommunityAndSocial { get; init; }
    public required int Interventions { get; init; }
    public required int Employment { get; init; }
    public required int TrainingAndEducation { get;init; }
}
