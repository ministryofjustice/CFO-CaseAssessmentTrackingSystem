using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Common.Enums;

public class TransferLocationType(string name, int value, IEnumerable<LocationType> fromTranslations, IEnumerable<LocationType> toTranslations) : SmartEnum<TransferLocationType>(name, value)
{
    public static readonly TransferLocationType CommunityToCommunity = new(
        name: "Community to Community", 
        value: 0, 
        fromTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }), 
        toTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }));

    public static readonly TransferLocationType CommunityToCustody = new(
        name: "Community to Custody",
        value: 1,
        fromTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }),
        toTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }));

    public static readonly TransferLocationType CustodyToCommunity = new(
        name: "Custody to Community",
        value: 2,
        fromTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }),
        toTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }));

    public static readonly TransferLocationType CustodyToCustody = new(
        name: "Custody to Custody",
        value: 3,
        fromTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }),
        toTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }));

    public static readonly TransferLocationType CustodyToUnmapped = new(
        name: "Custody to Unmapped",
        value: 4,
        fromTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }),
        toTranslations: LocationType.List.Where(location => location is { IsMapped: false }));

    public static readonly TransferLocationType CommunityToUnmapped = new(
        name: "Community to Unmapped",
        value: 5,
        fromTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }),
        toTranslations: LocationType.List.Where(location => location is { IsMapped: false }));

    public static readonly TransferLocationType UnmappedToCustody = new(
        name: "Unmapped to Custody",
        value: 6,
        fromTranslations: LocationType.List.Where(location => location is { IsMapped: false }),
        toTranslations: LocationType.List.Where(location => location is { IsCustody: true, IsMapped: true }));

    public static readonly TransferLocationType UnmappedToCommunity = new(
        name: "Unmapped to Community",
        value: 7,
        fromTranslations: LocationType.List.Where(location => location is { IsMapped: false }),
        toTranslations: LocationType.List.Where(location => location is { IsCommunity: true, IsMapped: true }));

    public static readonly TransferLocationType UnmappedToUnmapped = new(
        name: "Unmapped to Unmapped",
        value: 8,
        fromTranslations: LocationType.List.Where(location => location is { IsMapped: false }),
        toTranslations: LocationType.List.Where(location => location is { IsMapped: false }));

    public IEnumerable<LocationType> FromTranslations { get; private set; } = fromTranslations;
    public IEnumerable<LocationType> ToTranslations { get; private set; } = toTranslations;

    public static TransferLocationType DetermineFromLocationTypes(Location from, Location to)
    {
        var locationTypes = List
            .Where(tlt => tlt.FromTranslations.Contains(from.LocationType) 
                && tlt.ToTranslations.Contains(to.LocationType))
            .ToArray();

        if(locationTypes is not { Length: 1 })
        {
            throw new Exception($"Unsupported transfer type mapping configuration from {from.LocationType.Name} to {to.LocationType.Name}.");
        }

        return locationTypes.Single();
    }

}
