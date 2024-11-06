using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Common.Enums;

public class TransferLocationType(string name, int value) : SmartEnum<TransferLocationType>(name, value)
{
    public static readonly TransferLocationType CommunityToCommunity = new("Community to Community", 0);
    public static readonly TransferLocationType CommunityToCustody = new("Community to Custody", 1);
    public static readonly TransferLocationType CustodyToCommunity = new("Custody to Community", 2);
    public static readonly TransferLocationType CustodyToCustody = new("Custody to Custody", 3);

    public static TransferLocationType DetermineFromLocationTypes(Location from, Location to)
    {
        // IsFromCustody: false == Community
        return (IsFromCustody: from.LocationType.IsCustody, IsToCustody: to.LocationType.IsCustody) 
            switch
            {
                (IsFromCustody: false, IsToCustody: false) => CommunityToCommunity,
                (IsFromCustody: false, IsToCustody: true) => CommunityToCustody,
                (IsFromCustody: true, IsToCustody: false) => CustodyToCommunity,
                (IsFromCustody: true, IsToCustody: true) => CustodyToCustody
            };
    }

}
