using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Common.Exceptions;

public class InvalidEnrolmentTransition(EnrolmentStatus from, EnrolmentStatus to) 
    : Exception($"Participants cannot transition from {from.Name} to {to.Name}");
