using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchQueryHandler(
    ICandidateService candidateService,
    IUnitOfWork unitOfWork,
    IPrivilegedSearchAuditWriter privilegedSearchAuditWriter,
    ILogger<ParticipantAdvancedSearchQueryHandler> logger)
    : IQueryHandler<ParticipantAdvancedSearchQuery, Result<ParticipantAdvancedSearchResult>>
{
    public const int MaximumResults = 10;

    public async Task<Result<ParticipantAdvancedSearchResult>> Handle(
        ParticipantAdvancedSearchQuery request,
        CancellationToken cancellationToken)
    {
        var identifier = request.Identifier!.Trim().ToUpperInvariant();

        logger.LogInformation("Participant advanced search performed by {UserId}", request.CurrentUser.UserId);

        var participantQuery = unitOfWork.DbContext.Participants
            .AsNoTracking()
            .Include(p => p.Supervisor)
            .Include(p => p.CurrentLocation)
            .Include(p => p.ExternalIdentifiers);

        var participantMatches = await participantQuery
            .Where(p => p.Id == identifier || p.ExternalIdentifiers.Any(ei => ei.Value == identifier))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(MaximumResults + 1)
            .ToListAsync(cancellationToken);
        var participants = participantMatches.Take(MaximumResults).ToList();
        var truncated = participantMatches.Count > MaximumResults;

        var results = participants.Select(FromParticipant).ToList();
        var accessedCatsParticipantIds = participants.Select(p => p.Id).ToHashSet();
        var pendingAccessAuditParticipantIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var known = results.Select(r => r.Identifier).ToHashSet();
        var warnings = new List<string>();
        var searchResultUpcis = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await privilegedSearchAuditWriter.RecordAdvancedSearchAttemptAsync(
            request.CurrentUser,
            identifier,
            request.Justification,
            accessedCatsParticipantIds,
            cancellationToken);
        var userId = string.IsNullOrWhiteSpace(request.CurrentUser.UserId) ? Guid.Empty.ToString() : request.CurrentUser.UserId;

        // DMS search requires last name and date of birth; without them only CATS is searched.
        List<string> upcis = [];
        var dmsSearched = false;
        if (!string.IsNullOrWhiteSpace(request.LastName) && request.DateOfBirth is not null)
        {
            dmsSearched = true;
            var search = await candidateService.SearchAsync(new CandidateSearchQuery
            {
                ExternalIdentifier = identifier,
                FirstName = string.Empty,
                LastName = request.LastName.Trim(),
                DateOfBirth = request.DateOfBirth,
                CurrentUser = request.CurrentUser
            });

            if (!search.Succeeded && search is not NotFoundResult<SearchResult[]>)
            {
                warnings.Add(results.Count == 0
                    ? $"No participant found on CATS. DMS search could not be completed: {search.ErrorMessage}"
                    : $"DMS search could not be completed: {search.ErrorMessage}");
            }

            if (search is { Succeeded: true, Data: not null })
            {
                searchResultUpcis = search.Data
                    .Select(x => x.Upci)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                upcis = ParticipantAdvancedSearchMerge.SelectDmsUpcis(
                    search.Data,
                    known,
                    identifier,
                    MaximumResults + 1);
            }
            else if (search is NotFoundResult<SearchResult[]>)
            {
                upcis = ParticipantAdvancedSearchMerge.SelectDmsUpcis(
                    [],
                    known,
                    identifier,
                    MaximumResults + 1);
            }
        }
        
        var fallbackUpcis = upcis
            .Where(upci => string.Equals(upci, identifier, StringComparison.OrdinalIgnoreCase)
                && !searchResultUpcis.Contains(upci))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var dmsParticipants = upcis.Count == 0
            ? new Dictionary<string, Participant>()
            : await participantQuery
                .Where(p => upcis.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

        for (var i = 0; i < upcis.Count; i++)
        {
            var upci = upcis[i];

            if (dmsParticipants.TryGetValue(upci, out var participant))
            {
                if (results.Count >= MaximumResults)
                {
                    truncated = true;
                    break;
                }

                results.Add(FromParticipant(participant));
                if (accessedCatsParticipantIds.Add(participant.Id))
                {
                    pendingAccessAuditParticipantIds.Add(participant.Id);
                }
                continue;
            }

            var candidate = await candidateService.GetByUpciAsync(upci);

            if (candidate is { Succeeded: true, Data: not null })
            {
                if (fallbackUpcis.Contains(upci)
                    && !MatchesSearchCriteria(candidate.Data, request.LastName, request.DateOfBirth))
                {
                    continue;
                }

                if (results.Count >= MaximumResults)
                {
                    truncated = true;
                    break;
                }

                results.Add(ParticipantAdvancedSearchDto.From(candidate.Data));
            }
            else if (candidate is NotFoundResult<CandidateDto>)
            {
                if (searchResultUpcis.Contains(upci))
                {
                    warnings.Add($"DMS returned a match for result {upci}, but detailed record data was unavailable.");
                }
            }
            else
            {
                warnings.Add($"DMS details could not be loaded for result {upci}: {candidate.ErrorMessage}");
            }
        }

        foreach (var participantId in pendingAccessAuditParticipantIds)
        {
            unitOfWork.DbContext.AccessAuditTrails.Add(
                ParticipantAccessAuditTrail.Create(
                    nameof(ParticipantAdvancedSearchQuery),
                    participantId,
                    userId));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (truncated)
        {
            warnings.Add($"Search returned more than {MaximumResults} results. Showing the first {MaximumResults}.");
        }

        logger.LogInformation(
            "Participant advanced search by {UserId}. DMS searched: {DmsSearched}. Results: {ResultCount}. Warnings: {HasWarnings}",
            request.CurrentUser.UserId,
            dmsSearched,
            results.Count,
            warnings.Count > 0);

        return new ParticipantAdvancedSearchResult
        {
            Results = results.ToArray(),
            DmsSearched = dmsSearched,
            Warning = warnings.Count == 0 ? null : string.Join(" ", warnings)
        };
    }

    private static bool MatchesSearchCriteria(CandidateDto candidate, string? lastName, DateTime? dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(lastName) || dateOfBirth is null)
        {
            return true;
        }

        var dateMatches = candidate.DateOfBirth.Date == dateOfBirth.Value.Date;
        var surnameMatches = string.Equals(
            NormalizeSearchSurname(candidate.LastName),
            NormalizeSearchSurname(lastName),
            StringComparison.OrdinalIgnoreCase);
        return dateMatches && surnameMatches;
    }

    private static string NormalizeSearchSurname(string? value)
        => (value ?? string.Empty).Trim().Replace('’', '\'');

    private ParticipantAdvancedSearchDto FromParticipant(Participant p)
    {
        string? Identifier(ExternalIdentifierType type)
            => p.ExternalIdentifiers.FirstOrDefault(i => i.Type == type)?.Value;

        return new ParticipantAdvancedSearchDto
        {
            Identifier = p.Id,
            FirstName = p.FirstName,
            SecondName = p.MiddleName,
            LastName = p.LastName,
            DateOfBirth = p.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            Gender = p.Gender,
            NomisNumber = Identifier(ExternalIdentifierType.NomisNumber),
            Crn = Identifier(ExternalIdentifierType.Crn),
            PncNumber = Identifier(ExternalIdentifierType.PncNumber),
            PrimarySource = "CATS",
            LocationDescription = p.CurrentLocation.Name,
            IsInCustody = p.CurrentLocation.LocationType.IsCustody,
            CustodyLocation = p.CurrentLocation.LocationType.IsCustody ? p.CurrentLocation.Name : null,
            CommunityLocation = p.CurrentLocation.LocationType.IsCommunity ? p.CurrentLocation.Name : null,
            IsActive = p.IsActive(),
            IsParticipant = true,
            EnrolmentStatus = p.EnrolmentStatus?.Name,
            IsUnassigned = p.OwnerId is null,
            RegistrationDetails = ParseRegistrations(p.Id, p.RegistrationDetailsJson),
            SupervisorName = p.Supervisor?.Name,
            SupervisorTelephoneNumber = p.Supervisor?.TelephoneNumber,
            SupervisorMobileNumber = p.Supervisor?.MobileNumber,
            SupervisorEmailAddress = p.Supervisor?.EmailAddress,
            SupervisorAddress = p.Supervisor?.Address
        };
    }

    private string[] ParseRegistrations(string participantId, string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? [];
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to parse registration details for participant {ParticipantId}", participantId);
            return [];
        }
    }

}
