using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Domain.Events;

public sealed class PRICreatedDomainEvent(PRI pri) : CreatedDomainEvent<PRI>(pri);