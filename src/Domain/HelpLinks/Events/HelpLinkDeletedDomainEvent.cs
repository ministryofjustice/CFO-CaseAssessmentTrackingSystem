using Cfo.Cats.Domain.Common.Events;

namespace Cfo.Cats.Domain.HelpLinks.Events;

public sealed class HelpLinkDeletedDomainEvent(HelpLink entity) : DeletedDomainEvent<HelpLink>(entity);
