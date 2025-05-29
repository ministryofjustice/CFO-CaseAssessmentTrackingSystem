using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation;

public class InMemoryTargetsProviderReprofiled : ITargetsProvider
{
    private readonly ITargetsProvider _legacyTargetsProvider;
    private readonly Dictionary<string, ContractTargetDto> _targets;

    public InMemoryTargetsProviderReprofiled(ITargetsProvider legacyTargetsProvider)
    {
        _legacyTargetsProvider = legacyTargetsProvider;
        _targets = AddTargets();
    }

    public ContractTargetDto GetTarget(string contract, int month, int year)
    {
        var useLegacyTargetsProvider = IsBeforeReprofile(month, year);
        return useLegacyTargetsProvider ? _legacyTargetsProvider.GetTarget(contract, month, year) : _targets[contract];
    }

    private Dictionary<string, ContractTargetDto> AddTargets()
    {
        return new Dictionary<string, ContractTargetDto>
        {
            { "East Midlands", new ContractTargetDto("East Midlands", 102, 65, 5, 50, 56, 45, 501, 48, 90, 5, 15, 42) },
            {
                "East Of England",
                new ContractTargetDto("East Of England", 108, 80, 5, 50, 59, 47, 564, 48, 90, 5, 17, 47)
            },
            { "London", new ContractTargetDto("London", 96, 64, 5, 50, 53, 42, 480, 48, 90, 5, 14, 40) },
            { "North East", new ContractTargetDto("North East", 78, 98, 10, 100, 43, 34, 528, 95, 180, 10, 16, 44) },
            {
                "North West", new ContractTargetDto("North West", 162, 166, 10, 150, 89, 71, 984, 143, 270,
                    15, 30, 82)
            },
            { "South East", new ContractTargetDto("South East", 144, 89, 5, 50, 79, 63, 699, 48, 90, 5, 21, 58) },
            { "South West", new ContractTargetDto("South West", 111, 82, 3, 50, 61, 49, 579, 48, 90, 5, 17, 48) },
            {
                "West Midlands",
                new ContractTargetDto("South West", 126, 112, 8, 100, 69, 55, 714, 95, 180, 10, 21, 60)
            },
            {
                "Yorkshire and Humberside",
                new ContractTargetDto("Yorkshire and Humberside", 144, 161, 15, 150, 79, 63, 915, 143, 270, 15, 27, 76)
            }
        };
    }

    private bool IsBeforeReprofile(int month, int year)
    {
        DateTime cutOff = new(2025, 6, 1);
        DateTime askedFor = new(year, month, 1);
        return askedFor < cutOff;
    }
}