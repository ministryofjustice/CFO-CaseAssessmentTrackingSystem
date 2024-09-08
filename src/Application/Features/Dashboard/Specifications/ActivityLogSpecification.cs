using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Dashboard.Specifications;

public class ActivityLogSpecification : Specification<Timeline>
{
    public ActivityLogSpecification(ActivityLogFilter filter)
    {
        #nullable disable
        
        var today = DateTime.Now.ToUniversalTime().Date;



        Query.Where(t => t.Participant.OwnerId == filter.CurrentUser.UserId, filter.CurrentUser.AssignedRoles is []);

        Query.Where(t => t.Participant.Owner.TenantId.StartsWith(filter.CurrentUser.TenantId));
        
        Query.Where(
            p => p.Created >= DateTime.Now.Date,
            filter.ListView == ActivityLogListView.CreatedToday
            );
    }
}
