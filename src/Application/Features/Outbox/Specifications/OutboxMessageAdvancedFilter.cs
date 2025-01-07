using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Outbox.Specifications;

public class OutboxMessageAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
    public OutboxMessageListView ListView { get; set; } = OutboxMessageListView.All;
}