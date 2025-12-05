namespace Cfo.Cats.Domain.Common.Exceptions;

public class InvalidLabelNameException(string name) : DomainException($"Invalid label name: {name}");
public class InvalidLabelLengthException(string name) : DomainException($"{name} is an invalid length. It should be between 5 and 15 characters");
