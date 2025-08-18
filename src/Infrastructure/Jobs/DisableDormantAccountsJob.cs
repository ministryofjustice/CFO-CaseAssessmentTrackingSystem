using Cfo.Cats.Domain.Identity;
using Quartz;
using System.Linq.Dynamic.Core;

namespace Cfo.Cats.Infrastructure.Jobs;

public class DisableDormantAccountsJob(
                ILogger<DisableDormantAccountsJob> logger,
                IUnitOfWork unitOfWork
                ) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(DisableDormantAccountsJob));
    public static readonly string Description = "A job to deactivate accounts that have not logged in within the last 30 days.";

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
                logger.LogWarning($"Failed to complete Disable Dormant Accounts Job within 3 tries, aborting...");
                return;
            }
        }

        try
        {
            logger.LogInformation("Starting deactivation of dormant accounts");

            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var usersToDeactivate = await unitOfWork.DbContext.Users
                .IgnoreAutoIncludes()
                .Where(user => user.IsActive)
                .Where(user => user.LastLogin < thirtyDaysAgo ||
                               (user.LastLogin == null && user.Created < thirtyDaysAgo))
                .ToListAsync(context.CancellationToken);

            if (usersToDeactivate.Any())
            {
                var auditTrails = new List<IdentityAuditTrail>();

                foreach (var user in usersToDeactivate)
                {
                    user.IsActive = false;

                    var audit = IdentityAuditTrail.Create(
                        user.UserName,
                        "System.Support@justice.gov.uk",
                        IdentityActionType.AccountDeactivated,
                        "Overnight System Job");

                    auditTrails.Add(audit);
                    logger.LogInformation("Deactivated user {Id} account", user.Id);
                }

                await unitOfWork.DbContext.IdentityAuditTrails.AddRangeAsync(auditTrails, context.CancellationToken);
                await unitOfWork.SaveChangesAsync(context.CancellationToken);

                logger.LogInformation("Deactivated/Disabled {deactivated} account(s)", usersToDeactivate.Count);
            }
            else
            {
                logger.LogInformation($"No Accounts Disabled");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Quartz job {Key} failed", Key.Name);
            throw new JobExecutionException(msg: "An unexpected error occurred executing Disable Dormant Accounts job", refireImmediately: true, cause: ex);
        }
    }
}