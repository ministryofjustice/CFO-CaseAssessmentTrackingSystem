using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Events;

public sealed class InnovationFundCreatedDomainEvent(InnovationFund entity) : CreatedDomainEvent<InnovationFund>(entity);
