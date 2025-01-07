using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Outbox.Specifications;

public class OutboxMessageAdvancedSpecification : Specification<OutboxMessage>
{
    public OutboxMessageAdvancedSpecification(OutboxMessageAdvancedFilter filter)
    {
        var start = filter.ListView switch
        {
            // note: All gets ignored in the subsequent query. Here for branching
            // logic only.
            OutboxMessageListView.All => new DateTime(2024,8,1),
            OutboxMessageListView.CreatedToday => DateTime.Now.ToUniversalTime().Date,
            OutboxMessageListView.Last30Days => DateTime.Now.ToUniversalTime().Date.AddDays(-30),
            _ => throw new ArgumentOutOfRangeException()
        };


        Query.Where(p => p.OccurredOnUtc >= start, 
            filter.ListView is not OutboxMessageListView.All);

        Query.Where(p => p.Type.Contains(filter.Keyword!), string.IsNullOrEmpty(filter.Keyword) == false);

    }
}