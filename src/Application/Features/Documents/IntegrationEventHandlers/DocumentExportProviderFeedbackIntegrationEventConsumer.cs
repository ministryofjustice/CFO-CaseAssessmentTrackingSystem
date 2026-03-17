using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Documents;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Documents.IntegrationEventHandlers;

public class DocumentExportProviderFeedbackIntegrationEventConsumer(
    IUnitOfWork unitOfWork,
    IExcelService excelService,
    IUploadService uploadService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<DocumentExportProviderFeedbackIntegrationEventConsumer> logger) : IHandleMessages<ExportDocumentIntegrationEvent>
{
    public async Task Handle(ExportDocumentIntegrationEvent context)
    {
        if (context.Key != DocumentTemplate.ProviderFeedback.Name)
        {
            logger.LogDebug("Export document not supported by this handler");
            return;
        }

        var document = await unitOfWork.DbContext.GeneratedDocuments.FindAsync(context.DocumentId);

        if (document is null)
        {
            logger.LogError("Export provider feedback document event raised for a document that does not exist. ({DocumentId})", context.DocumentId);
            return;
        }

        try
        {
            var request = JsonConvert.DeserializeObject<ExportProviderFeedback.ProviderFeedbackExportRequest>(context.SearchCriteria!)
                ?? throw new Exception("Failed to deserialise export request.");

            // Stub user profile — handlers below do not use CurrentUser in their query body.
            var stubUser = new UserProfile { UserName = "system", Email = "system@system", UserId = context.UserId };

            var sheets = new List<(string SheetName, byte[] Data)>();

            if (request.IncludeEnrolmentReturns)
            {
                var query = new GetEnrolmentsToProvider.Query
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TenantId = request.TenantId,
                    CurrentUser = stubUser
                };
                var data = await new GetEnrolmentsToProvider.Handler(unitOfWork).Handle(query, CancellationToken.None);
                if (data is not { Succeeded: true })
                {
                    throw new Exception(data.ErrorMessage);
                }

                var sheet = await excelService.ExportAsync(data.Data!.TabularData,
                    new Dictionary<string, Func<GetEnrolmentsToProvider.EnrolmentsTabularData, object?>>
                    {
                        { "Contract",           r => r.ContractName },
                        { "Queue",              r => r.Queue },
                        { "Participant",        r => r.ParticipantId },
                        { "Support Worker",     r => r.SupportWorker },
                        { "Provider QA",        r => r.PqaUser },
                        { "CFO User",           r => r.CfoUser },
                        { "PQA Submitted Date", r => r.PqaSubmittedDate },
                        { "Returned Date",      r => r.ReturnedDate },
                        { "Advisory Notes",     r => r.Message },
                    });
                sheets.Add(("Enrolment Returns", sheet));
            }

            if (request.IncludeActivitiesReturns)
            {
                var query = new GetActivitiesToProvider.Query
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TenantId = request.TenantId,
                    CurrentUser = stubUser
                };
                var data = await new GetActivitiesToProvider.Handler(unitOfWork).Handle(query, CancellationToken.None);
                if (data is not { Succeeded: true })
                {
                    throw new Exception(data.ErrorMessage);
                }

                var sheet = await excelService.ExportAsync(data.Data!.TabularData,
                    new Dictionary<string, Func<GetActivitiesToProvider.ActivitiesTabularData, object?>>
                    {
                        { "Contract",           r => r.ContractName },
                        { "Queue",              r => r.Queue },
                        { "Activity Type",      r => r.ActivityType?.Name },
                        { "Participant",        r => r.ParticipantId },
                        { "Support Worker",     r => r.SupportWorker },
                        { "Provider QA",        r => r.PqaUser },
                        { "CFO User",           r => r.CfoUser },
                        { "PQA Submitted Date", r => r.PqaSubmittedDate },
                        { "Returned Date",      r => r.ReturnedDate },
                        { "Advisory Notes",     r => r.Message },
                    });
                sheets.Add(("Activities Returns", sheet));
            }

            if (request.IncludeEnrolmentAdvisories)
            {
                var query = new GetEnrolmentAdvisoriesToProvider.Query
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TenantId = request.TenantId,
                    CurrentUser = stubUser
                };
                var data = await new GetEnrolmentAdvisoriesToProvider.Handler(unitOfWork).Handle(query, CancellationToken.None);
                if (data is not { Succeeded: true })
                {
                    throw new Exception(data.ErrorMessage);
                }

                var sheet = await excelService.ExportAsync(data.Data!.TabularData,
                    new Dictionary<string, Func<GetEnrolmentAdvisoriesToProvider.EnrolmentAdvisoriesTabularData, object?>>
                    {
                        { "Contract",           r => r.ContractName },
                        { "Queue",              r => r.Queue },
                        { "Participant",        r => r.ParticipantId },
                        { "Support Worker",     r => r.SupportWorker },
                        { "Provider QA",        r => r.PqaUser },
                        { "CFO User",           r => r.CfoUser },
                        { "PQA Submitted Date", r => r.PqaSubmittedDate },
                        { "Advisory Date",      r => r.AdvisoryDate },
                        { "Feedback Type",      r => r.FeedbackType.HasValue ? ((FeedbackType)r.FeedbackType.Value).ToString() : null },
                        { "Advisory Notes",     r => r.Message },
                    });
                sheets.Add(("Enrolment Advisories", sheet));
            }

            if (request.IncludeActivitiesAdvisories)
            {
                var query = new GetActivitiesAdvisoriesToProvider.Query
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TenantId = request.TenantId,
                    CurrentUser = stubUser
                };
                var data = await new GetActivitiesAdvisoriesToProvider.Handler(unitOfWork).Handle(query, CancellationToken.None);
                if (data is not { Succeeded: true })
                {
                    throw new Exception(data.ErrorMessage);
                }

                var sheet = await excelService.ExportAsync(data.Data!.TabularData,
                    new Dictionary<string, Func<GetActivitiesAdvisoriesToProvider.ActivitiesAdvisoriesTabularData, object?>>
                    {
                        { "Contract",           r => r.ContractName },
                        { "Queue",              r => r.Queue },
                        { "Activity Type",      r => r.ActivityType?.Name },
                        { "Participant",        r => r.ParticipantId },
                        { "Support Worker",     r => r.SupportWorker },
                        { "Provider QA",        r => r.PqaUser },
                        { "CFO User",           r => r.CfoUser },
                        { "PQA Submitted Date", r => r.PqaSubmittedDate },
                        { "Advisory Date",      r => r.AdvisoryDate },
                        { "Feedback Type",      r => r.FeedbackType.HasValue ? ((FeedbackType)r.FeedbackType.Value).ToString() : null },
                        { "Advisory Notes",     r => r.Message },
                    });
                sheets.Add(("Activities Advisories", sheet));
            }

            var merged = await excelService.MergeSheetsAsync(sheets.ToArray());

            var uploadRequest = new UploadRequest(document.Title!, UploadType.Document, merged);
            var result = await uploadService.UploadAsync($"MyDocuments/{context.UserId}", uploadRequest);

            if (result.Succeeded)
            {
                document.WithStatus(DocumentStatus.Available).SetURL(result);
            }
            else
            {
                logger.LogError("Failed to upload provider feedback document {DocumentId}: {Errors}", context.DocumentId, string.Join(", ", result.Errors));
                document.WithStatus(DocumentStatus.Error);
            }

            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error exporting provider feedback document {DocumentId}: {ErrorMessage}", context.DocumentId, ex.Message);
            document.WithStatus(DocumentStatus.Error);
            await unitOfWork.CommitTransactionAsync();
        }
    }
}
