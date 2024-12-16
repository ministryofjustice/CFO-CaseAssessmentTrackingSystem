using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Payables.Queries
{
    public class ActivityQueueEntryFilter
    : PaginationFilter
    {
        public UserProfile? CurrentUser { get; set; }
    }
}