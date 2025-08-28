using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Rebus.Handlers;
using System.Runtime.Intrinsics.Arm;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class GenerateOutcomQualityDipSampleDocument(IUnitOfWork unitOfWork, 
    IDomainEventDispatcher domainEventDispatcher, 
    ILogger<GenerateOutcomQualityDipSampleDocument> logger) : IHandleMessages<OutcomeQualityDipSampleFinalisedIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleFinalisedIntegrationEvent message)
    {
        try
        {
            // we need some additional information from the database

            var db = unitOfWork.DbContext;

            var contractQuery = from dp in db.OutcomeQualityDipSamples
                          join c in db.Contracts on dp.ContractId equals c.Id
                          where dp.Id == message.DipSampleId
                          select new
                          {
                              c.Description,
                              From = dp.PeriodFrom.Date,
                              To = dp.PeriodTo.Date,
                          };

            var userQuery = from u in db.Users
                            where u.Id == message.UserId
                            select u.TenantId;

            var tenantId = await userQuery.AsNoTracking().FirstAsync();
            var contract = await contractQuery.AsNoTracking().FirstAsync();

            await unitOfWork.BeginTransactionAsync();
            var document = GeneratedDocument.Create(
                  template: DocumentTemplate.OutcomeQualityDipSample,
                  title: $"{contract.Description} Outcome Quality ({contract.From.ToString("MMM yyyy")}).xlsx",
                  description: $"Outcome quality dipsample results for {contract.Description}. Covering {contract.From.ToShortDateString()} to {contract.To.ToShortDateString()}",
                  createdBy: message.UserId,
                  tenantId: tenantId,
                  message.DipSampleId.ToString()
                  );

            unitOfWork.DbContext.GeneratedDocuments.Add(document);
            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, default);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating dipsample document");
            await unitOfWork.RollbackTransactionAsync();
        }

    }
}
