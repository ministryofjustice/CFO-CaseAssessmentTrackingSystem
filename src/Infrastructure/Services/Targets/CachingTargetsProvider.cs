
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Targets;

public class CachingTargetsProvider(
    IFusionCache cache,
    ITargetsProvider targetsProvider,
    ILogger<CachingTargetsProvider> logger) : ITargetsProvider
{

    private const string Tag = "Contract-Targets";

    public IReadOnlyList<ContractTargetDto> DataSet
    {
        get
        {
            var cached = cache.TryGet<IReadOnlyList<ContractTargetDto>>(Tag);

            if (cached.HasValue)
            {
                return cached.Value;
            }

            logger.LogDebug("No cached entry found. Fetching from DB");

            var result = targetsProvider.DataSet;
            cache.Set(Tag, result, TimeSpan.FromDays(1), tags: [Tag]);

            return result;
        }
    }

    public ContractTargetDto GetTarget(string contract, int month, int year)
    {
        var target = from ct in DataSet
                        where ct.Contract.Equals(contract, StringComparison.CurrentCultureIgnoreCase)
                            && ct.Year == year
                            && ct.Month == month
                        select ct;

        return target.SingleOrDefault(ContractTargetDto.EmptyTarget(contract));
    }

    public ContractTargetDto GetTargetById(string contractId, int month, int year)
    {
        var target = from ct in DataSet
                        where ct.ContractId.Equals(contractId, StringComparison.CurrentCultureIgnoreCase)
                            && ct.Year == year
                            && ct.Month == month
                        select ct;

        return target.SingleOrDefault(ContractTargetDto.EmptyTarget(contractId));
    }

    public void Refresh()
    {
        logger.LogInformation("Clearing targets cache");
        cache.Remove(Tag);
    }

}