using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class MappaLevel : SmartEnum<MappaLevel>
{
    public static readonly MappaLevel Level1 = new(nameof(Level1), "Multi-agency support is where the risks posed by the individual can be managed by the lead agency in co-operation with other agencies but without the need for formal multi-agency meetings. Responsible Authority and Duty to Co-operate agencies have a statutory obligation to engage with MAPPA at all levels, including Level 1, and will be involved in the management of the individual as necessary.", 1);
    public static readonly MappaLevel Level2 = new(nameof(Level2), "Active multi-agency management is where the ongoing involvement of several agencies is needed to manage the individual. Once at Level 2, there will be regular multi-agency public protection meetings about the individual.", 2);
    public static readonly MappaLevel Level3 = new(nameof(Level3), "Enhanced multi-agency management is more demanding on resources and requires the involvement of senior people from the agencies, who can authorise the use of extra resources, for example, surveillance or emergency accommodation.", 3);

    private MappaLevel(string name, string description, int value) 
        : base(name, value) 
    {
        Description = description;
    }

    public string Description { get; private set; }
}
