namespace Cfo.Cats.Domain.Common.Contracts;

public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}