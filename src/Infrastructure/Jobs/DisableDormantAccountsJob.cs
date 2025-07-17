using Cfo.Cats.Domain.Identity;
using FluentValidation;
using Quartz;
using System.Linq.Dynamic.Core;

namespace Cfo.Cats.Infrastructure.Jobs;

public class DisableDormantAccountsJob(
                ILogger<DisableDormantAccountsJob> logger,
                UserManager<ApplicationUser> userManager,
                IUnitOfWork unitOfWork
                ) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(DisableDormantAccountsJob));
    public static readonly string Description = "A job to deactivate accounts that have not logged in within the last 30 days.";

    public async Task Execute(IJobExecutionContext context)
    {

        using (logger.BeginScope(Key))

            if (context.RefireCount > 3)
            {
                logger.LogWarning($"Failed to complete Disable Dormant Accounts Job within 3 tries, aborting...");
                return;
            }

        try
        {
            logger.LogInformation("Starting deactivation of dormant accounts");

            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var usersToDeactivate = userManager.Users
                .IgnoreAutoIncludes()
                .Where(user => user.IsActive)
                .Where(user => user.LastLogin < thirtyDaysAgo ||
                               (user.LastLogin == null && user.Created < thirtyDaysAgo));

            if (usersToDeactivate.Any())
            {
                foreach (var user in usersToDeactivate)
                {
                    await DeactivateUser(user, context.CancellationToken);
                }

                int noOfAffectedUsers = await usersToDeactivate.ExecuteUpdateAsync(
                    p => p.SetProperty(user => user.IsActive, false), context.CancellationToken);

                logger.LogInformation($"Deactivated/Disabled {noOfAffectedUsers} account(s)");
            }
            else
            {
                logger.LogInformation($"No Accounts Disabled");
            }
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"An unexpected error occurred executing Disable Dormant Accounts job", refireImmediately: true, cause: ex);
        }
    }

    private async Task DeactivateUser(ApplicationUser user, CancellationToken cancellationToken)
    {
        const string systemSupportEmail = "System.Support@justice.gov.uk";

        IdentityAuditTrail audit = IdentityAuditTrail.Create(user.UserName, systemSupportEmail, IdentityActionType.AccountDeactivated, "Overnight System Job");
        await unitOfWork.DbContext.IdentityAuditTrails.AddAsync(audit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"Deactivated {user.UserName} account");
        await Task.CompletedTask;
    }
}