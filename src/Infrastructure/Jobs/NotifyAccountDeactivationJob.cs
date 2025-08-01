using Cfo.Cats.Domain.Identity;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

public class NotifyAccountDeactivationJob(
    ILogger<NotifyAccountDeactivationJob> logger,
    UserManager<ApplicationUser> userManager,
    ICommunicationsService communicationsService) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(NotifyAccountDeactivationJob));
    public static readonly string Description = "A job to notify accounts that are due to deactivate.";
    public static readonly DateTime sevenDaysFromDeactivationDate = DateTime.Today.AddDays(-23); // exactly 23 days ago, ignoring time

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
                ["JobName"] = Key.Name,
                ["JobGroup"] = Key.Group ?? "Default",
                ["JobInstance"] = Guid.NewGuid().ToString()
        }))

        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Failed to complete notifying accounts that are due to deactivate within 3 tries, aborting...");
            return;
        }

        try
        {
            logger.LogInformation("Starting notifying accounts that will be deactivated soon");               

            var users = await userManager.Users
                .IgnoreAutoIncludes()
                .Where(user => user.IsActive)
                .Where(user =>
                       user.LastLogin.HasValue
                     ? user.LastLogin.Value.Date == sevenDaysFromDeactivationDate
                     : user.Created.HasValue && user.Created.Value.Date == sevenDaysFromDeactivationDate)
                .ToListAsync();

            if (users.Any())
            {
                users.ForEach(Notify);

                var userList = string.Join(", ", users.Select(u => u.UserName));
                logger.LogInformation("Following users warned they will be deactivated soon: {Users}", userList);                
            }
            else
            {
                logger.LogInformation($"No accounts need notifying");
            }

            async void Notify(ApplicationUser user) => await communicationsService.SendAccountDeactivationEmail(user.Email!);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"An unexpected error occurred executing notifying accounts that will be deactivated soon job", refireImmediately: true, cause: ex);
        }
    }
}