namespace Cfo.Cats.Infrastructure.Jobs;

public class JobOptions
{
    public bool Enabled { get; set; }
    public string CronSchedule { get; set; } = string.Empty;
}
