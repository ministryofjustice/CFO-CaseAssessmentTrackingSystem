using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class LocationType : SmartEnum<LocationType>
{
    public static readonly LocationType Wing = new(nameof(Wing), 0, isCustody: true);
    public static readonly LocationType Feeder = new(nameof(Feeder), 1, isCustody: true);
    public static readonly LocationType Outlying = new(nameof(Outlying), 2, isCustody: true);
    public static readonly LocationType Female = new(nameof(Female), 3, isCustody: true);
    public static readonly LocationType Community = new(nameof(Community), 4, isCommunity: true);
    public static readonly LocationType Hub = new(nameof(Hub), 5, isCommunity: true, isHub: true);
    public static readonly LocationType Satellite = new(nameof(Satellite), 6, isCommunity: true, isHub: true);
    public static readonly LocationType Unknown = new(nameof(Unknown), 7, isMapped: false);
    public static readonly LocationType UnmappedCustody = new("Unmapped Custody", 8, isCustody: true, isMapped: false);
    public static readonly LocationType UnmappedCommunity = new("Unmapped Community", 9, isCommunity: true, isMapped: false); 

    private LocationType(string name, int value, bool isCustody = false, bool isCommunity = false, bool isHub = false, bool isMapped = true) : base(name, value)
    {
        if(isCustody is true && isCommunity is true)
        {
            throw new ArgumentException($"Invalid configuration for location type '{name}'.");
        }

        if(isHub is true && isCommunity is false)
        {
            throw new ArgumentException($"Invalid configuration for hub location type '{name}'.");
        }

        IsCustody = isCustody;
        IsCommunity = isCommunity;
        IsHub = isHub;
        IsMapped = isMapped;

    }

    public bool IsCustody { get; private set; }
    public bool IsCommunity { get; private set; }
    public bool IsHub { get; private set; }
    public bool IsMapped { get; private set; }
}