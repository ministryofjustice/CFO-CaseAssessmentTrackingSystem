using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Enrolments.Specifications;

public class QueueEntryFilter
    : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
}
