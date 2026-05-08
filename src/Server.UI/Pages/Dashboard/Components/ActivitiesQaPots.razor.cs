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

        var unitOfWork = GetNewUnitOfWork();

        var context = unitOfWork.DbContext;
        
        _educationCfo = await GetCfoActivityCountByType(ActivityType.EducationAndTraining, context);
        _employmentCfo = await GetCfoActivityCountByType(ActivityType.Employment, context);
        _supportWorkCfo = await GetCfoActivityCountByType(ActivityType.SupportWork, context);

        _educationPqa = await GetPqaActivityCountByType(ActivityType.EducationAndTraining, context);
        _employmentPqa = await GetPqaActivityCountByType(ActivityType.Employment, context);
        _supportWorkPqa = await GetPqaActivityCountByType(ActivityType.SupportWork, context);
        
        _loading = false;
    }

    private async Task<int> GetPqaActivityCountByType(
        ActivityType type,
        IApplicationDbContext context) => await (
            from q in context.ActivityPqaQueue.AsNoTracking()
            join a in context.Activities.AsNoTracking() on q.ActivityId equals a.Id
            where !q.IsCompleted
                  && a.OwnerId == UserProfile.UserId
                  && a.Type == type
            select q
        ).CountAsync();

    private async Task<int> GetCfoActivityCountByType(
        ActivityType type,
        IApplicationDbContext context)
    {
        var qa1 =
            from q in context.ActivityQa1Queue.AsNoTracking()
            join a in context.Activities.AsNoTracking() on q.ActivityId equals a.Id
            where !q.IsCompleted
                  && a.OwnerId == UserProfile.UserId
                  && a.Type == type
            select q.ActivityId;

        var qa2 =
            from q in context.ActivityQa2Queue.AsNoTracking()
            join a in context.Activities.AsNoTracking() on q.ActivityId equals a.Id
            where !q.IsCompleted
                  && a.OwnerId == UserProfile.UserId
                  && a.Type == type
            select q.ActivityId;

        var escalation =
            from q in context.ActivityEscalationQueue.AsNoTracking()
            join a in context.Activities.AsNoTracking() on q.ActivityId equals a.Id
            where !q.IsCompleted
                  && a.OwnerId == UserProfile.UserId
                  && a.Type == type
            select q.ActivityId;

        return await qa1
            .Concat(qa2)
            .Concat(escalation)
            .CountAsync();
    }
}