using FluentValidation.Internal;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IValidatorStrategy<T>
{
    Action<ValidationStrategy<T>> Strategy { get; }
}
