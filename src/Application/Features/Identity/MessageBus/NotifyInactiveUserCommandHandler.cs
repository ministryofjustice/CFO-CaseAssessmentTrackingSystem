using Cfo.Cats.Domain.Entities.Participants;
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
        logger.LogDebug($"Notifying inactive user with email: {context.Email}");

        try
        {
            using var scope = logger.BeginScope("Notify inactive user: {Email}", context.Email);

            await communicationsService.SendAccountDeactivationEmail(context.Email);

            logger.LogDebug("Finished notifying inactive user with email: {Email}", context.Email);
        }

        catch (Exception e)
        {
            logger.LogError(e, "Failed to notify inactive user with email: {Email}", context.Email);
            await unitOfWork.RollbackTransactionAsync();
        }
           
            
    }
}