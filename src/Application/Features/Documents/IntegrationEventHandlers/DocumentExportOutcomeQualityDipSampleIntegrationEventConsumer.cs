using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Documents;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportOutcomeQualityDipSampleIntegrationEventConsumer
    (
        IUnitOfWork unitOfWork,
        IOutcomeQualityDipSampleExcelService excelService,
        IUploadService uploadService,
        IDomainEventDispatcher domainEventDispatcher,
        ILogger<DocumentExportOutcomeQualityDipSampleIntegrationEventConsumer> logger
    )
      : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent message)
    {
        if(message.Key != DocumentTemplate.OutcomeQualityDipSample.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(message.DocumentId);

        if(document == null)
        {
            logger.LogError("Export document event raised for a document that does not exist. ({DocumentId})", message.DocumentId);
            return;
        }

        if(document.SearchCriteriaUsed is null)
        {
            logger.LogError("Document with id {DocumentId} has no search criteria", message.DocumentId);
            return;
        }

        try
        {
            var dipSampleId = Guid.Parse(document.SearchCriteriaUsed);

            var db = unitOfWork.DbContext;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var query = from ds in db.OutcomeQualityDipSamples
                        join c in db.Contracts on ds.ContractId equals c.Id
                        join cpm in db.Users on ds.VerifiedBy equals cpm.Id
                        where ds.Id == dipSampleId
                        select new
                        {
                            Region = c.Description,
                            CPM = cpm.DisplayName,
                            Score = ds.FinalScore,
                            Participants = (
                                from sp in db.OutcomeQualityDipSampleParticipants
                                join p in db.Participants on sp.ParticipantId equals p.Id
                                where sp.DipSampleId == dipSampleId
                                select new
                                {
                                    Participant = $"{p.FirstName} {p.LastName} ({p.Id})",
                                    Type = sp.LocationType,
                                    CurrentLocation = p.CurrentLocation.Name,
                                    EnrolledAt = p.EnrolmentLocation!.Name,
                                    SupportWorker = p.Owner.DisplayName,
                                    Compliant = sp.FinalIsCompliant.IsAccepted,
                                    CsoComments = sp.CsoComments,
                                    CpmComments = sp.CpmComments,
                                    FinalComments = sp.FinalComments
                                }
                            ).ToArray()
                        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var summary = await query.FirstOrDefaultAsync();

            if (summary == null) 
            {
                logger.LogError("Document with id {DocumentId} references a dip sample {DipSampleId} that does not exist", message.DocumentId, dipSampleId);
                return;
            }

            excelService.WithDipSampleSummary(summary.Region, DateTime.Now.Date, summary.CPM, summary.Score!.Value);

            foreach (var p in summary.Participants) 
            {
                excelService.AddParticipant(p.Participant, p.Type, p.CurrentLocation, p.EnrolledAt, p.SupportWorker, p.Compliant, p.CsoComments, p.CpmComments, p.FinalComments);
            }

            var results = await excelService.ExportAsync();
            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, results);

            var result = await uploadService.UploadAsync($"MyDocuments/{message.UserId}", uploadRequest);

            if (result.Succeeded)
            {
                document.WithStatus(DocumentStatus.Available)
                    .SetURL(result);
            }
            else
            {
                logger.LogError("Failed to upload document {DocumentId}: {Errors}", message.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();

        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error exporting outcome quality dip sample document {DocumentId}", message.DocumentId);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }

    }
}
