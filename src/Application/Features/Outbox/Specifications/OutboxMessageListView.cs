namespace Cfo.Cats.Application.Features.Outbox.Specifications;

public enum OutboxMessageListView
{
    [Description("All")]
    All,
    [Description("Created Today")]
    CreatedToday,
    [Description("Last 30 days")]
    Last30Days

}