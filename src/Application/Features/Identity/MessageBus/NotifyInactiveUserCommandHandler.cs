using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.Identity.MessageBus;

public class NotifyInactiveUserCommandHandler(
    IUnitOfWork unitOfWork,
    ICommunicationsService communicationsService,
    ILogger<NotifyInactiveUserCommandHandler> logger)
    : IHandleMessages<NotifyInactiveUserCommand>
{
    public async Task Handle(NotifyInactiveUserCommand context)
    {
        logger.LogDebug("Notifying inactive user with email: {Email}", context.Email);

        try
        {
            using var scope = logger.BeginScope("Notify inactive user: {Email}", context.Email);

            var sendResult = await communicationsService.SendAccountDeactivationEmail(context.Email);

            if (sendResult.Succeeded is false)
            {
                // NotifyAccountDeactivationJob publishes each user exactly once (on the day they hit
                // the 23-day threshold), so ack-ing a failed send would mean the reminder is never
                // delivered. Fail the delivery instead so it follows the same path as any other
                // failure (see catch block below).
                throw new InvalidOperationException(
                    $"Account deactivation email could not be sent: {sendResult.ErrorMessage}");
            }

            logger.LogDebug("Finished notifying inactive user with email: {Email}", context.Email);
        }

        catch (Exception e)
        {
            logger.LogError(e, "Failed to notify inactive user with email: {Email}", context.Email);
            try
            {
                await unitOfWork.RollbackTransactionAsync();
            }
            catch (Exception rollbackException)
            {
                logger.LogError(
                    rollbackException,
                    "Failed to rollback transaction after notify inactive user failure for email: {Email}",
                    context.Email);
            }

            // Intentional change from the original swallow-and-rollback behaviour: re-throw so Rebus
            // treats this delivery as failed instead of ack-ing it. Rebus then redelivers up to
            // RabbitSettings.Retries total attempts (currently 1 in appsettings, i.e. no retry)
            // and moves the message to Rebus's default "error" queue, where it can be inspected and
            // replayed (e.g. once Notify configuration has been fixed).
            throw;
        }
    }
}