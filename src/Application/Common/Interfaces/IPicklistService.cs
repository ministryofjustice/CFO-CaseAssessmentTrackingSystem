using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IPicklistService
{
    List<KeyValueDto> DataSource { get; }
    event Action? OnChange;
    void Initialize();
    void Refresh();
}