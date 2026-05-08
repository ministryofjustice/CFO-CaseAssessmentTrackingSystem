using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class ActivitiesQaPots
{
    private bool _loading;

    private int _educationCfo;
    private int _employmentCfo;
    private int _supportWorkCfo;

    private int _educationPqa;
    private int _employmentPqa;
    private int _supportWorkPqa;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        
        var userId = UserProfile.UserId;
        
        var unitOfWork = GetNewUnitOfWork();

        var context = unitOfWork.DbContext;

        var cfo = await GetCfoCounts(context, userId);
        var pqa = await GetPqaCounts(context, userId);

        _educationCfo = cfo.GetValueOrDefault(ActivityType.EducationAndTraining);
        _employmentCfo = cfo.GetValueOrDefault(ActivityType.Employment);
        _supportWorkCfo = cfo.GetValueOrDefault(ActivityType.InterventionsAndServicesWraparoundSupport);

        _educationPqa = pqa.GetValueOrDefault(ActivityType.EducationAndTraining);
        _employmentPqa = pqa.GetValueOrDefault(ActivityType.Employment);
        _supportWorkPqa = pqa.GetValueOrDefault(ActivityType.InterventionsAndServicesWraparoundSupport);

        _loading = false;
    }

    private async Task<Dictionary<ActivityType, int>> GetPqaCounts(
        IApplicationDbContext context,
        string userId) => await (
                from q in context.ActivityPqaQueue.AsNoTracking()
                join a in context.Activities.AsNoTracking()
                    on q.ActivityId equals a.Id
                where !q.IsCompleted
                      && a.OwnerId == userId
                group q by a.Type
                into grouped
                select new
                {
                    grouped.Key,
                    Count = grouped.Count()
                }
            )
            .ToDictionaryAsync(x => x.Key, x => x.Count);

    private async Task<Dictionary<ActivityType, int>> GetCfoCounts(
        IApplicationDbContext context,
        string userId)
    {
        var qa1 =
            from q in context.ActivityQa1Queue.AsNoTracking()
            where !q.IsCompleted
            select q.ActivityId;

        var qa2 =
            from q in context.ActivityQa2Queue.AsNoTracking()
            where !q.IsCompleted
            select q.ActivityId;

        var escalation =
            from q in context.ActivityEscalationQueue.AsNoTracking()
            where !q.IsCompleted
            select q.ActivityId;

        var combined = qa1
            .Concat(qa2)
            .Concat(escalation);

        return await (
                from activityId in combined
                join a in context.Activities.AsNoTracking()
                    on activityId equals a.Id
                where a.OwnerId == userId
                group activityId by a.Type
                into grouped
                select new
                {
                    grouped.Key,
                    Count = grouped.Count()
                }
            )
            .ToDictionaryAsync(x => x.Key, x => x.Count);
    }
}