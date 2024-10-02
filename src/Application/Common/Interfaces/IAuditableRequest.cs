namespace Cfo.Cats.Application.Common.Interfaces;

public interface IAuditableRequest<out TResponse> 
    : IRequest<TResponse>
{
    string Identifier();
}

