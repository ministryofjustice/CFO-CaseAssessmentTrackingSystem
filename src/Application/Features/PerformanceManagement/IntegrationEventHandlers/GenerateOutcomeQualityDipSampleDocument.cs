using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEventHandlers;

public class GenerateOutcomeQualityDipSampleDocument(IUnitOfWork unitOfWork, 
    IDomainEventDispatcher domainEventDispatcher, 
    ILogger<GenerateOutcomeQualityDipSampleDocument> logger) : IHandleMessages<OutcomeQualityDipSampleFinalisedIntegrationEvent>
{
    public async Task Handle(OutcomeQualityDipSampleFinalisedIntegrationEvent message)
    {
        try
        {
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

            var ds = await unitOfWork.DbContext.OutcomeQualityDipSamples
                .FirstAsync(i => i.Id == message.DipSampleId);

            ds.WithDocument(document.Id);

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
