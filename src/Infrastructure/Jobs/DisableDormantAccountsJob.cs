using Cfo.Cats.Domain.Identity;
using Quartz;
using System.Linq.Dynamic.Core;

namespace Cfo.Cats.Infrastructure.Jobs;

public class DisableDormantAccountsJob(
    ILogger<DisableDormantAccountsJob> logger,
    UserManager<ApplicationUser> userManager) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(DisableDormantAccountsJob));
    public static readonly string Description = "A job to deactivate accounts that have not logged in within the last 30 days.";

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(Key))

        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Failed to complete within 3 tries, aborting...");
            return;
        }

        try
        {
            logger.LogInformation("Deactivating dormant accounts");

            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var query = userManager.Users
                .IgnoreAutoIncludes()
                .Where(user => user.IsActive)
                .Where(user => user.LastLogin < thirtyDaysAgo
                    || (user.LastLogin == null && user.Created < thirtyDaysAgo));

            int noOfAffectedUsers = await query.ExecuteUpdateAsync(
                p => p.SetProperty(user => user.IsActive, false));

            logger.LogInformation($"Deactivated {noOfAffectedUsers} account(s)");
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"An unexpected error occurred executing job", refireImmediately: true, cause: ex);
        }

    }
}
