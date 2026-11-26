using System.Security.Cryptography;
using System.Text;
using Cfo.Cats.Application.Features.Identity.MessageBus;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Common.Enums;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class NotifyAccountDeactivationJob(
    ILogger<NotifyAccountDeactivationJob> logger,
    IUnitOfWork unitOfWork,
    TimeProvider? timeProvider = null) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(NotifyAccountDeactivationJob));
    public static readonly string Description = "A job to notify accounts that are due to deactivate.";

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
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
                logger.LogWarning($"Failed to complete notifying accounts that are due to deactivate within 3 tries, aborting...");
                return;
            }
        }

        try
        {
            logger.LogInformation("Starting notifying accounts that will be deactivated soon");               

            var clock = timeProvider ?? TimeProvider.System;
            var activityBefore = clock.GetLocalNow().Date.AddDays(-22);

            var users = await unitOfWork.DbContext.Users
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .Where(user => user.Status == UserStatus.Active)
                .Where(user =>
                       user.LastLogin.HasValue
                     ? user.LastLogin.Value < activityBefore
                     : user.Created.HasValue && user.Created.Value < activityBefore)
                .OrderBy(user => user.Id)
                .ToListAsync(cancellationToken);

            foreach(var user in users)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var activityAt = (user.LastLogin ?? user.Created)!.Value;
                var identity = FormattableString.Invariant($"{nameof(NotifyInactiveUserCommand)}:{user.Id}:{activityAt.Ticks}");
                var messageId = new Guid(SHA256.HashData(Encoding.UTF8.GetBytes(identity)).AsSpan(0, 16));
                if (await unitOfWork.DbContext.OutboxMessages.AnyAsync(message => message.Id == messageId, cancellationToken))
                {
                    continue;
                }

                var command = new NotifyInactiveUserCommand(user.Email!)
                {
                    MessageId = messageId
                };
                await unitOfWork.DbContext.InsertOutboxMessage(
                    command,
                    outboxMessageId: messageId,
                    occurredOnUtc: clock.GetUtcNow().UtcDateTime);
                try
                {
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch
                {
                    unitOfWork.DbContext.ChangeTracker.Clear();
                    throw;
                }
            }

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new JobExecutionException($"An unexpected error occurred executing notifying accounts that will be deactivated soon job", ex) { RefireImmediately = true };
        }
    }
}