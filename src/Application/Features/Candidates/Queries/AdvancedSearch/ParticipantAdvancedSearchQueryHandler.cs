using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchQueryHandler(
    ICandidateService candidateService,
    IUnitOfWork unitOfWork,
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

        var participants = await participantQuery
            .Where(p => p.Id == identifier || p.ExternalIdentifiers.Any(ei => ei.Value == identifier))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(MaximumResults)
            .ToListAsync(cancellationToken);

        var results = participants.Select(FromParticipant).ToList();
        var known = results.Select(r => r.Identifier).ToHashSet();
        var warnings = new List<string>();

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
                upcis = ParticipantAdvancedSearchMerge.SelectDmsUpcis(
                    search.Data,
                    known,
                    identifier,
                    MaximumResults);
            }
        }

        var dmsParticipants = upcis.Count == 0
            ? new Dictionary<string, Participant>()
            : await participantQuery
                .Where(p => upcis.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var upci in upcis)
        {
            if (results.Count >= MaximumResults)
            {
                break;
            }

            if (dmsParticipants.TryGetValue(upci, out var participant))
            {
                results.Add(FromParticipant(participant));
                continue;
            }

            var candidate = await candidateService.GetByUpciAsync(upci);

            if (candidate is { Succeeded: true, Data: not null })
            {
                results.Add(ParticipantAdvancedSearchDto.From(candidate.Data));
            }
            else if (candidate is not NotFoundResult<CandidateDto>)
            {
                warnings.Add($"DMS details could not be loaded for result {upci}: {candidate.ErrorMessage}");
            }
        }

        // Searching across all participants and DMS is privileged, so record who searched for what.
        // Name and date of birth are deliberately not logged.
        logger.LogInformation(
            "Participant advanced search by {UserId} for {SearchIdentifier}. DMS searched: {DmsSearched}. Results: {ResultCount}. Warnings: {HasWarnings}",
            request.CurrentUser.UserId,
            identifier,
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

    private static ParticipantAdvancedSearchDto FromParticipant(Participant p)
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
            RegistrationDetails = ParseRegistrations(p.RegistrationDetailsJson),
            SupervisorName = p.Supervisor?.Name,
            SupervisorTelephoneNumber = p.Supervisor?.TelephoneNumber,
            SupervisorMobileNumber = p.Supervisor?.MobileNumber,
            SupervisorEmailAddress = p.Supervisor?.EmailAddress,
            SupervisorAddress = p.Supervisor?.Address
        };
    }

    private static string[] ParseRegistrations(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
