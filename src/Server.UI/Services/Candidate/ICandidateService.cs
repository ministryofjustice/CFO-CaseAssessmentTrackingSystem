﻿using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Server.UI.Services.Candidate;

public interface ICandidateService
{
    public Task<IEnumerable<SearchResult>?> SearchAsync(CandidateSearchQuery searchQuery);
    public Task<CandidateDto?> GetByUpciAsync(string upci);
}

public record SearchResult(string Upci, int Precedence);