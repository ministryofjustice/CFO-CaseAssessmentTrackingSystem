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

        var participants = await unitOfWork.DbContext.Participants
            .AsNoTracking()
            .Include(p => p.Supervisor)
            .Include(p => p.CurrentLocation)
            .Include(p => p.ExternalIdentifiers)
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
        if (!string.IsNullOrWhiteSpace(request.LastName) && request.DateOfBirth is not null)
        {
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

        foreach (var upci in upcis)
        {
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

        return new ParticipantAdvancedSearchResult
        {
            Results = results.Take(MaximumResults).ToArray(),
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
