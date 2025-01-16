using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation;

public interface ITargetsProvider
{
    public ContractTargetDto GetTarget(string contract, int month, int year);
}

public class InMemoryTargetsProvider : ITargetsProvider
{

    private Dictionary<string, ContractTargetDto> targets;

    public InMemoryTargetsProvider()
    {
        targets = AddTargets();
    }

    private Dictionary<string, ContractTargetDto> AddTargets()
    {
        return new Dictionary<string, ContractTargetDto> {
            { "East Midlands",      new ContractTargetDto("East Midlands",          102,    65, 5,  50, 82, 57, 501,    48, 90, 5,  15, 42)},
            { "East Of England",        new ContractTargetDto("East Of England",        108,    80, 5,  50, 86, 60, 564,    48, 90, 5,  17, 47)},
            { "London",             new ContractTargetDto("London",             96, 64, 5,  50, 77, 54, 480,    48, 90, 5,  14, 40)},
            { "North East",         new ContractTargetDto("North East",         78, 98, 10, 100,    62, 43, 528,    95, 180,    10, 16, 44)},
            { "North West",         new ContractTargetDto("North West",         162,    166,    10, 150,    130,    91, 984,    143,    270,    15, 30, 82)},
            { "South East",         new ContractTargetDto("South East",         144,    89, 5,  50, 115,    81, 699,    48, 90, 5,  21, 58)},
            { "South West",         new ContractTargetDto("South West",         111,    82, 3,  50, 89, 63, 579,    48, 90, 5,  17, 48)},
            { "West Midlands",      new ContractTargetDto("South West",         126,    112,    8,  100,    101,    71, 714,    95, 180,    10, 21, 60)},
            { "Yorkshire and Humberside",   new ContractTargetDto("Yorkshire and Humberside",   144,    161,    15, 150,    115,    81, 915,    143,    270,    15, 27, 76)},

        };
    }

    public ContractTargetDto GetTarget(string contract, int month, int year) => targets[contract];
}