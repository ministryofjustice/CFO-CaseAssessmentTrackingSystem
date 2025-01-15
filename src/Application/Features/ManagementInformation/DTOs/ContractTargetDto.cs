using System.Diagnostics.CodeAnalysis;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation.DTOs;

public record ContractTargetDto
{
    public ContractTargetDto()
    {
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
    public int TrainingAndEducation { get;init; }
}
