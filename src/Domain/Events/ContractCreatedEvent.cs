using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Events;

public class ContractCreatedEvent(Contract entity) : CreatedEvent<Contract>(entity);