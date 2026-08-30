using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

public class DeleteNotificationsJob(IUnitOfWork unitOfWork, ILogger<DeleteNotificationsJob> logger) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(DeleteNotificationsJob));
    public static readonly string Description = "A job to delete notifications that are older than 30 days.";

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
                ["JobName"] = Key.Name,
                ["JobGroup"] = Key.Group ?? "Default",
                ["JobInstance"] = Guid.NewGuid().ToString()
        }))
        {
            if (context.RefireCount > 3)
            {
                logger.LogWarning($"Failed to complete delete notifications that are due within 3 tries, aborting...");
                return;
            }

            try
            {
                logger.LogInformation("Starting deletion of old notifications");

                var cutOff = DateTime.Now.Date.AddDays(-30);

                var affected = await unitOfWork.DbContext.Notifications
                        .Where(n => n.Created < cutOff)
                        .ExecuteDeleteAsync();

                logger.LogInformation("Completed deletion of old notifications. {Deleted} notifications deleted.", affected);

            }
            catch(Exception ex)
            {
                throw new JobExecutionException(msg: $"An unexpected error occurred running delete notifications job", refireImmediately: true, cause: ex);
            }
        }
    }
}