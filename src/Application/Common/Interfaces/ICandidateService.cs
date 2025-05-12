using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface ICandidateService
{
    public Task<Result<SearchResult[]>> SearchAsync(CandidateSearchQuery searchQuery);
    public Task<Result<CandidateDto>> GetByUpciAsync(string upci);

    public Task<Result<bool>> SetStickyLocation(string upci, string location);
}

public record SearchResult(string Upci, int Precedence);