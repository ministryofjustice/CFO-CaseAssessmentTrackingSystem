using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Common.Exceptions;

public class InvalidDipSampleTransitionException : DomainException
{
    public InvalidDipSampleTransitionException(DipSampleStatus from, DipSampleStatus to) 
        : base($"Sample cannot transition from {from.Name} to {to.Name}")
    {
    }

    public InvalidDipSampleTransitionException(string message)
        : base(message)
    {

    }
}

public class MissingParticipantDetailsException() 
    : DomainException("Cannot call this method without loading the participants");

