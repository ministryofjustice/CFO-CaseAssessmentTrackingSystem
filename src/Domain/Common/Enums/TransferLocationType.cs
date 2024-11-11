using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Common.Enums;

public class TransferLocationType(string name, int value) : SmartEnum<TransferLocationType>(name, value)
{
    public static readonly TransferLocationType CommunityToCommunity = new("Community to Community", 0);
    public static readonly TransferLocationType CommunityToCustody = new("Community to Custody", 1);
    public static readonly TransferLocationType CustodyToCommunity = new("Custody to Community", 2);
    public static readonly TransferLocationType CustodyToCustody = new("Custody to Custody", 3);
    public static readonly TransferLocationType CustodyToUnmapped = new("Custody to Unmapped", 4);
    public static readonly TransferLocationType CommunityToUnmapped = new("Community to Unmapped", 5);
    public static readonly TransferLocationType UnmappedToCustody = new("Unmapped to Custody", 6);
    public static readonly TransferLocationType UnmappedToCommunity = new("Unmapped to Community", 7);
    public static readonly TransferLocationType UnmappedToUnmapped = new("Unmapped to Unmapped", 8);

    public static TransferLocationType DetermineFromLocationTypes(Location from, Location to)
    {
        bool fromIsUnmapped = from.LocationType == LocationType.UnmappedCustody ||
                            from.LocationType == LocationType.UnmappedCommunity ||
                            from.LocationType == LocationType.Unknown;

        bool toIsUnmapped = to.LocationType == LocationType.UnmappedCommunity ||
                            to.LocationType == LocationType.UnmappedCustody ||
                            to.LocationType == LocationType.Unknown;

        if (fromIsUnmapped)
        {
            if (toIsUnmapped)
            {
                return UnmappedToUnmapped;
            }
            else if (to.LocationType.IsCustody)
            {
                return UnmappedToCustody;
            }
            else if (to.LocationType.IsCommunity)
            {
                return UnmappedToCommunity;
            }
        }
        else if (toIsUnmapped)
        {
            if (from.LocationType.IsCustody)
            {
                return CustodyToUnmapped;
            }
            else if (from.LocationType.IsCommunity)
            {
                return CommunityToUnmapped;
            }
        }
        else if (to.LocationType.IsCommunity)
        {
            if (from.LocationType.IsCustody)
            {
                return CustodyToCommunity;
            }
            else if (from.LocationType.IsCommunity)
            {
                return CommunityToCommunity;
            }
        }
        else if (to.LocationType.IsCustody)
        {
            if (from.LocationType.IsCustody)
            {
                return CustodyToCustody;
            }
            else if (from.LocationType.IsCommunity)
            {
                return CommunityToCustody;
            }
        }

        return UnmappedToUnmapped;
    }
}
