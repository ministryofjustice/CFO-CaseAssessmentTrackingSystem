using Cfo.Cats.Application.Common.MessageBus;

namespace Cfo.Cats.Application.Features.Identity.MessageBus;

public record NotifyInactiveUserCommand(string Email) : IExternalCommand;