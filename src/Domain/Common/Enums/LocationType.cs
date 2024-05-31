using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Common.Enums;

public class LocationType : SmartEnum<LocationType>
{
    
    public bool IsCustody { get; private set; }
    
    private LocationType(string name, int value, bool isCustody = false) : base(name, value)
    {
    }
    
    public static readonly LocationType Wing = new LocationType(nameof(Wing), 0, true);
    public static readonly LocationType Feeder = new LocationType(nameof(Feeder), 1, true);
    public static readonly LocationType Outlying = new LocationType(nameof(Outlying), 2, true);
    public static readonly LocationType Unspecified = new LocationType(nameof(Unspecified), 3, true);
    public static readonly LocationType Community = new LocationType(nameof(Community), 4);
    public static readonly LocationType Hub = new LocationType(nameof(Hub), 5);
    public static readonly LocationType Satellite = new LocationType(nameof(Satellite), 5);
    
}

