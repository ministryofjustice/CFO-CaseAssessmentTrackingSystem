using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class LocationType : SmartEnum<LocationType>
{

    public static readonly LocationType Wing = new(nameof(Wing), 0, true);
    public static readonly LocationType Feeder = new(nameof(Feeder), 1, true);
    public static readonly LocationType Outlying = new(nameof(Outlying), 2, true);
    public static readonly LocationType Unspecified = new(nameof(Unspecified), 3, true);
    public static readonly LocationType Community = new(nameof(Community), 4);
    public static readonly LocationType Hub = new(nameof(Hub), 5, isHub: true);
    public static readonly LocationType Satellite = new(nameof(Satellite), 6, isHub: true);
    public static readonly LocationType Unknown = new(nameof(Unknown), 6, false); 

    private LocationType(string name, int value, bool isCustody = false, bool isHub = false) : base(name, value)
    {
        IsCustody = isCustody;
        IsHub = isHub;
    }
    public bool IsCustody { get; private set; }
    public bool IsHub { get; private set; }
}
