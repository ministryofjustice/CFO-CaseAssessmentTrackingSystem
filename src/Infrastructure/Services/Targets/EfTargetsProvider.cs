using System.Diagnostics.Contracts;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.ManagementInformation.DTOs;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Microsoft.Extensions.DependencyInjection;

namespace Cfo.Cats.Infrastructure.Services.Targets;

public class EfTargetsProvider(IServiceScopeFactory scopeFactory) : ITargetsProvider
{
    public IReadOnlyList<ContractTargetDto> DataSet
    {
        get
        {
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var query = from c in context.Contracts
                        join ct in context.ContractTargets on c.Id equals ct.ContractId
                        select new ContractTargetDto
                        {
                            ContractId = c.Id,
                            Month = ct.Month,
                            Year = ct.Year,
                            Community = ct.Community,
                            CommunityAndSocial = ct.CommunityAndSocial,
                            Contract = c.Description,
                            Employment = ct.Employment,
                            Hubs = ct.Hubs,
                            HumanCitizenship = ct.HumanCitizenship,
                            Interventions = ct.Interventions,
                            PreReleaseSupport = ct.PreReleaseSupport,
                            Prison = ct.Prison,
                            SupportWork = ct.SupportWork,
                            ThroughTheGate = ct.ThroughTheGate,
                            TrainingAndEducation = ct.TrainingAndEducation,
                            Wings = ct.Wings
                        };

            return query.ToList().AsReadOnly();
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
        // do nothing. we do not cache at the database level
    }
}