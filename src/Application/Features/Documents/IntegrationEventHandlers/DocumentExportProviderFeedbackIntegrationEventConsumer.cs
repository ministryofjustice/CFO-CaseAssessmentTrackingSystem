using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Documents.IntegrationEvents;
using Cfo.Cats.Application.SecurityConstants;
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

            // Stub user profile for handlers that require a CurrentUser context. Access to this
            // consumer's exports is already gated by the SeniorInternal/Internal policy on the
            // commands that trigger it (ExportProviderFeedback / ExportMyFeedback).
            var stubUser = new UserProfile
            {
                UserName = "system",
                Email = "system@system",
                UserId = context.UserId,
                TenantId = context.TenantId,
                AssignedRoles = [RoleNames.SystemSupport]
            };

            var justMySheets = string.IsNullOrWhiteSpace(request.UserId) == false;

            (string SheetName, byte[] Data)[] sheets;

            if(justMySheets)
            {
                sheets =
                [
                    await BuildEnrolmentFeedbackSheet(request, stubUser),
                    await BuildActivitiesFeedbackSheet(request, stubUser),
                ];
            }
            else
            {
                sheets =
                [
                    await BuildEnrolmentReturnsSheet(request, stubUser),
                    await BuildActivitiesReturnsSheet(request, stubUser),
                    await BuildEnrolmentAdvisoriesSheet(request, stubUser),
                    await BuildActivitiesAdvisoriesSheet(request, stubUser),
                    await BuildEnrolmentFeedbackSheet(request, stubUser),
                    await BuildActivitiesFeedbackSheet(request, stubUser),
                ];
            }

            var merged = await excelService.MergeSheetsAsync(sheets);

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

    private async Task<(string SheetName, byte[] Data)> BuildEnrolmentReturnsSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
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
                { "Return Reason",      r => r.ReturnReason },
                { "Returned Date",      r => r.ReturnedDate },
                { "Advisory Notes",     r => r.Message },
            });
        return ("Enrolment Returns", sheet);
    }

    private async Task<(string SheetName, byte[] Data)> BuildActivitiesReturnsSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
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
                { "Return Reason",      r => r.ReturnReason },
                { "Returned Date",      r => r.ReturnedDate },
                { "Advisory Notes",     r => r.Message },
            });
        return ("Activities Returns", sheet);
    }

    private async Task<(string SheetName, byte[] Data)> BuildEnrolmentAdvisoriesSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
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
        return ("Enrolment Advisories", sheet);
    }

    private async Task<(string SheetName, byte[] Data)> BuildActivitiesAdvisoriesSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
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
        return ("Activities Advisories", sheet);
    }

    private async Task<(string SheetName, byte[] Data)> BuildEnrolmentFeedbackSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
    {
        var query = new GetEnrolmentsFeedback.Query
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TenantId = request.TenantId,
            UserId = request.UserId,
            CurrentUser = stubUser
        };
        var data = await new GetEnrolmentsFeedback.Handler(unitOfWork).Handle(query, CancellationToken.None);
        if (data is not { Succeeded: true })
        {
            throw new Exception(data.ErrorMessage);
        }

        // Mirrors the columns shown on the corresponding screen: "My Feedback" does not
        // display who gave the feedback or the recipient - only "Team Feedback" does.
        var justMySheet = string.IsNullOrWhiteSpace(request.UserId) == false;

        var mappers = new Dictionary<string, Func<GetEnrolmentsFeedback.EnrolmentsFeedbackTabularData, object?>>
        {
            { "Participant",                r => r.ParticipantId },
            { "Queue",                      r => r.Queue },
        };

        if (justMySheet is false)
        {
            mappers.Add("Recipient", r => r.RecipientUser);
            mappers.Add("QA User", r => r.QaUser);
        }

        mappers.Add("QA1 Outcome", r => r.Qa1Outcome);
        mappers.Add("Outcome", r => r.Outcome);
        mappers.Add("Feedback Reason", r => r.EnrolmentFeedbackReason);
        mappers.Add("Enrolment Processed Date", r => r.EnrolmentProcessedDate.ToShortDateString());
        mappers.Add("Feedback Date", r => r.Created.HasValue ? r.Created.Value.ToShortDateString() : null);
        mappers.Add("Message", r => r.Message);
        mappers.Add("Read", r => r.IsRead);

        var sheet = await excelService.ExportAsync(data.Data!.TabularData, mappers);

        var sheetName = justMySheet ? "My Enrolment Feedback" : "Enrolment Team Feedback";
        return (sheetName, sheet);
    }

    private async Task<(string SheetName, byte[] Data)> BuildActivitiesFeedbackSheet(
        ExportProviderFeedback.ProviderFeedbackExportRequest request, UserProfile stubUser)
    {
        var query = new GetActivitiesFeedback.Query
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TenantId = request.TenantId,
            UserId = request.UserId,
            CurrentUser = stubUser
        };
        var data = await new GetActivitiesFeedback.Handler(unitOfWork).Handle(query, CancellationToken.None);
        if (data is not { Succeeded: true })
        {
            throw new Exception(data.ErrorMessage);
        }

        // Mirrors the columns shown on the corresponding screen: "My Feedback" does not
        // display who gave the feedback or the recipient - only "Team Feedback" does.
        var justMySheet = string.IsNullOrWhiteSpace(request.UserId) == false;

        var mappers = new Dictionary<string, Func<GetActivitiesFeedback.ActivitiesFeedbackTabularData, object?>>
        {
            { "Participant",       r => r.ParticipantId },
            { "Activity Category", r => r.ActivityCategory },
            { "Activity Type",     r => r.ActivityType },
            { "Queue",             r => r.Queue },
        };

        if (justMySheet is false)
        {
            mappers.Add("Recipient", r => r.RecipientUser);
            mappers.Add("QA User", r => r.QaUser);
        }

        mappers.Add("QA1 Outcome", r => r.Qa1Outcome);
        mappers.Add("Outcome", r => r.Outcome);
        mappers.Add("Feedback Reason", r => r.ActivityFeedbackReason);
        mappers.Add("Activity Processed Date", r => r.ActivityProcessedDate.ToShortDateString());
        mappers.Add("Feedback Date", r => r.Created.HasValue ? r.Created.Value.ToShortDateString() : null);
        mappers.Add("Message", r => r.Message);
        mappers.Add("Read", r => r.IsRead);

        var sheet = await excelService.ExportAsync(data.Data!.TabularData, mappers);

        var sheetName = justMySheet ? "My Activities Feedback" : "Activities Team Feedback";
        return (sheetName, sheet);
    }
}

