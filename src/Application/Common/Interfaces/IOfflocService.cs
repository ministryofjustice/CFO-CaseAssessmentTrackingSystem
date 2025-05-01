using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IOfflocService
{
    Task<Result<PersonalDetailsDto>> GetPersonalDetailsAsync(string nomisNumber);
}