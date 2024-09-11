using Cfo.Cats.Domain.Identity;
using DocumentFormat.OpenXml.Spreadsheet;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

public class NotifyAccountDeactivationJob(
    ILogger<NotifyAccountDeactivationJob> logger,
    UserManager<ApplicationUser> userManager,
    ICommunicationsService communicationsService) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(NotifyAccountDeactivationJob));
    public static readonly string Description = "A job to notify accounts that are due to deactivate.";

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
            logger.LogInformation("Notifying accounts that will be deactivated soon");

            DateTime sevenDaysFromDeactivationDate = DateTime.Now
                .AddDays(-30) // 30 days ago
                .AddDays(7); // 7 days before

            var users = await userManager.Users
                .IgnoreAutoIncludes()
                .Where(user => user.IsActive)
                .Where(user => user.LastLogin < sevenDaysFromDeactivationDate
                    || (user.LastLogin == null && user.Created < sevenDaysFromDeactivationDate))
                .ToListAsync();

            users.ForEach(Notify);

            async void Notify(ApplicationUser user) => await communicationsService.SendAccountDeactivationEmail(user.Email!);
        }
        catch(Exception ex)
        {
            throw new JobExecutionException(msg: $"An unexpected error occurred executing job", refireImmediately: true, cause: ex);
        }

    }

}
