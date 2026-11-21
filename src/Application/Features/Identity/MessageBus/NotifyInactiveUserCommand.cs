using Cfo.Cats.Application.Common.MessageBus;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Identity.MessageBus;

public record NotifyInactiveUserCommand(string Email) : IntegrationEvent(DateTime.UtcNow), IExternalCommand;