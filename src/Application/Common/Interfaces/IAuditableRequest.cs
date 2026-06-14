namespace Cfo.Cats.Application.Common.Interfaces;

public interface IAuditableRequest<out TResponse> 
    : IQuery<TResponse>
{
    string Identifier();
}
