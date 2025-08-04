namespace Cfo.Cats.Infrastructure.Jobs;

public class JobOptions
{
    public bool Enabled { get; set; }
    public Schedule[] CronSchedules { get; set; } = [];
}

public class Schedule
{
    public string Chron { get; set; } = default!;
    public string Description { get; set; } = default!;
}