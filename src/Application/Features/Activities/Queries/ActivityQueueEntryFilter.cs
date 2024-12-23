using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Activities.Queries
{
    public class ActivityQueueEntryFilter
    : PaginationFilter
    {
        public UserProfile? CurrentUser { get; set; }
    }
}