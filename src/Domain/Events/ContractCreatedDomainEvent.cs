using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Events;

public sealed class ContractCreatedDomainEvent(Contract entity) : CreatedDomainEvent<Contract>(entity);