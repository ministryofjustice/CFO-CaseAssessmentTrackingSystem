using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events;

public sealed class EducationTrainingActivityCreatedDomainEvent(EducationTrainingActivity entity) : CreatedDomainEvent<EducationTrainingActivity>(entity);
public sealed class EmploymentActivityCreatedDomainEvent(EmploymentActivity entity) : CreatedDomainEvent<EmploymentActivity>(entity);
public sealed class ISWActivityCreatedDomainEvent(ISWActivity entity) : CreatedDomainEvent<ISWActivity>(entity);
public sealed class NonISWActivityCreatedDomainEvent(NonISWActivity entity) : CreatedDomainEvent<NonISWActivity>(entity);
