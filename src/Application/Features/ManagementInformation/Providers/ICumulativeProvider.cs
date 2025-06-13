using Cfo.Cats.Application.Features.ManagementInformation.DTOs;

namespace Cfo.Cats.Application.Features.ManagementInformation.Providers;

public interface ICumulativeProvider
{
    Task<Actuals> GetActuals(DateOnly startDate, DateOnly endDate, string contractId);
    Task<Actuals> GetActuals(DateOnly startDate, DateOnly endDate, string[] contracts);
    
    Task<ContractTargetDto> GetTargets(DateOnly startDate, DateOnly endDate, string contractId);
    
    Task<ContractTargetDto> GetTargets(DateOnly startDate, DateOnly endDate, string[] contractId);
}

public class CumulativeProvider(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : ICumulativeProvider
{
    public async Task<Actuals> GetActuals(DateOnly startDate, DateOnly endDate, string contractId)
    {
        var all = await GetActuals(startDate, endDate);
        return all.First(a => a.contract_id.Equals(contractId, StringComparison.OrdinalIgnoreCase));
    }
    public async Task<Actuals> GetActuals(DateOnly startDate, DateOnly endDate, string[] contracts)
    {
        var all = await GetActuals(startDate, endDate);
        return all
            .Where(c => contracts.Contains(c.contract_id))
            .Aggregate(
            new Actuals("All", "All", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            (acc, x) => acc with
            {
                custody_enrolments = acc.custody_enrolments + x.custody_enrolments,
                community_enrolments = acc.community_enrolments + x.community_enrolments,
                wing_inductions = acc.wing_inductions + x.wing_inductions,
                hub_inductions = acc.hub_inductions + x.hub_inductions,
                prerelease_support = acc.prerelease_support + x.prerelease_support,
                ttg = acc.ttg + x.ttg,
                support_work = acc.support_work + x.support_work,
                human_citizenship = acc.human_citizenship + x.human_citizenship,
                community_and_social = acc.community_and_social + x.community_and_social,
                isws = acc.isws + x.isws,
                employment = acc.employment + x.employment,
                education = acc.education + x.education
            });
    }
    public Task<ContractTargetDto> GetTargets(DateOnly startDate, DateOnly endDate, string contractId) => GetTargets(startDate, endDate, [contractId]);
    public Task<ContractTargetDto> GetTargets(DateOnly startDate, DateOnly endDate, string[] contracts)
    {
        var monthlyDates = Enumerable.Range(0, int.MaxValue)
            .Select(startDate.AddMonths)
            .TakeWhile(date => date <= endDate)
            .ToArray();
        
        ContractTargetDto target = new ContractTargetDto("", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        
        foreach (var contract in contracts)
        {
            foreach (var month in monthlyDates)
            {
                target += targetsProvider.GetTargetById(contract, month.Month, month.Year);
            }
        }
        
        // This is a task because in future we will move targets into the database.
        return Task.FromResult(target);
    }

    private async Task<Actuals[]> GetActuals(DateOnly startDate, DateOnly endDate)
    {
        var results = await unitOfWork.DbContext.Database
            .SqlQuery<Actuals>($"SELECT * FROM mi.GetCumulativeTotals({startDate}, {endDate})")
            .ToArrayAsync();
        
        return results;
    }
}