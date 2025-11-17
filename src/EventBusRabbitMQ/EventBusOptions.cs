namespace Cfo.Cats.EventBusRabbitMQ;

public class EventBusOptions
{
    public required string SubscriptionClientName { get; set; }
    
    /// <summary>
    /// The amount of retries to attempt. Defaults to 3 
    /// </summary>
    public int  RetryCount { get; set; } = 3;
    
    /// <summary>
    /// The prefetch count. Defaults to 1
    /// </summary>
    public ushort PrefetchCount { get; set; } = 1;
}