namespace Cfo.Cats.Infrastructure.Services.MessageHandling;

public class OvernightServiceSettings
{
    public int Workers { get; set; } = 5;
    public int Parallelism { get; set; } = 5;
}