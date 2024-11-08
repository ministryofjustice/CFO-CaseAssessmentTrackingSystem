using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class LocationType : SmartEnum<LocationType>
{

    public static readonly LocationType Wing = new(nameof(Wing), 0, isCustody: true);
    public static readonly LocationType Feeder = new(nameof(Feeder), 1, isCustody: true);
    public static readonly LocationType Outlying = new(nameof(Outlying), 2, isCustody: true);
    public static readonly LocationType Unspecified = new(nameof(Unspecified), 3, isCustody: true);
    public static readonly LocationType Community = new(nameof(Community), 4, isCommunity: true);
    public static readonly LocationType Hub = new(nameof(Hub), 5, isCommunity: true, isHub: true);
    public static readonly LocationType Satellite = new(nameof(Satellite), 6, isCommunity: true, isHub: true);
    public static readonly LocationType Unknown = new(nameof(Unknown), 6);
    public static readonly LocationType UnmappedCustody = new(nameof(UnmappedCustody), 7, isCustody: true);
    public static readonly LocationType UnmappedCommunity = new(nameof(UnmappedCommunity), 8, isCommunity: true); 

    private LocationType(string name, int value, bool isCustody = false, bool isCommunity = false, bool isHub = false) : base(name, value)
    {
        if(isCustody is true && isCommunity is true)
        {
            throw new ArgumentException("Invalid configuration for location type");
        }

        if(isHub is true && isCommunity is false)
        {
            throw new ArgumentException("Invalid configuration for hub location type");
        }

        IsCustody = isCustody;
        IsCommunity = isCommunity;
        IsHub = isHub;

    }

    public bool IsCustody { get; private set; }
    public bool IsCommunity { get; private set; }
    public bool IsHub { get; private set; }
}
