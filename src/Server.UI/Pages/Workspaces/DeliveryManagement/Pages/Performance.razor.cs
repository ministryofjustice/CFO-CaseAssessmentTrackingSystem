using Cfo.Cats.Application.Features.Dashboard.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Performance
{
    private MudDateRangePicker _picker = null!;

    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    private string PerformanceKey => $"{SelectionKey}|{_dateRange.Start?.Ticks ?? 0}|{_dateRange.End?.Ticks ?? 0}";

    /// <summary>
    /// Performance has 7 independent tabs (Enrolments, Inductions, Support &amp; Referral, Activities,
    /// Education &amp; Training, Employment, Reassessments), each backed by its own dashboard query with
    /// its own idea of "assignee" (participant owner vs. activity owner vs. payment support worker).
    /// Rather than one generic combined query, each tab's own dedicated *Assignees query is called
    /// separately and the results are merged purely for the single shared picker/label on this page.
    /// </summary>
    protected override async Task<IDictionary<string, string>> LoadUsersAsync()
    {
        var mediator = GetNewMediator();

        var enrolmentsTask = mediator.Send(new GetEnrolmentAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var inductionsTask = mediator.Send(new GetInductionAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var supportReferralsTask = mediator.Send(new GetSupportReferralAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var reassessmentsTask = mediator.Send(new GetReassessmentAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var paidActivitiesTask = mediator.Send(new GetPaidActivityAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var educationAndTrainingTask = mediator.Send(new GetEducationAndTrainingAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });
        var employmentsTask = mediator.Send(new GetEmploymentAssignees.Query(CurrentUser) { TenantId = SelectedTenantId });

        await Task.WhenAll(enrolmentsTask, inductionsTask, supportReferralsTask, reassessmentsTask,
            paidActivitiesTask, educationAndTrainingTask, employmentsTask);

        var users = new Dictionary<string, string>();

        var enrolments = await enrolmentsTask;
        if (enrolments is { Succeeded: true, Data: not null })
        {
            foreach (var a in enrolments.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var inductions = await inductionsTask;
        if (inductions is { Succeeded: true, Data: not null })
        {
            foreach (var a in inductions.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var supportReferrals = await supportReferralsTask;
        if (supportReferrals is { Succeeded: true, Data: not null })
        {
            foreach (var a in supportReferrals.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var reassessments = await reassessmentsTask;
        if (reassessments is { Succeeded: true, Data: not null })
        {
            foreach (var a in reassessments.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var paidActivities = await paidActivitiesTask;
        if (paidActivities is { Succeeded: true, Data: not null })
        {
            foreach (var a in paidActivities.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var educationAndTraining = await educationAndTrainingTask;
        if (educationAndTraining is { Succeeded: true, Data: not null })
        {
            foreach (var a in educationAndTraining.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        var employments = await employmentsTask;
        if (employments is { Succeeded: true, Data: not null })
        {
            foreach (var a in employments.Data)
            {
                users[a.Id] = a.DisplayName;
            }
        }

        return users;
    }
}

