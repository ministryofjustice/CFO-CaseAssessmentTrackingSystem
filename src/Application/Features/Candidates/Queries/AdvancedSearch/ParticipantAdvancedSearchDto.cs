using Cfo.Cats.Application.Features.Candidates.DTOs;

namespace Cfo.Cats.Application.Features.Candidates.Queries.AdvancedSearch;

public class ParticipantAdvancedSearchDto
{
    // Name
    public required string Identifier { get; init; }
    public required string FirstName { get; init; }
    public string? SecondName { get; init; }
    public required string LastName { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string? Gender { get; init; }

    // Identifiers
    public string? NomisNumber { get; init; }
    public string? Crn { get; init; }
    public string? PncNumber { get; init; }
    public string? PrimarySource { get; init; }

    // Location
    public string? LocationDescription { get; init; }
    public string? CustodyLocation { get; init; }
    public string? CommunityLocation { get; init; }
    public bool IsInCustody { get; init; }

    // Status
    public bool IsActive { get; init; }
    public string[] RegistrationDetails { get; init; } = [];
    public bool IsParticipant { get; init; }

    /// <summary>
    /// Enrolment status on CATS (participants only).
    /// </summary>
    public string? EnrolmentStatus { get; init; }
    public bool IsUnassigned { get; init; }

    // Offender manager / supervisor held locally on CATS (participants only)
    public string? SupervisorName { get; init; }
    public string? SupervisorTelephoneNumber { get; init; }
    public string? SupervisorMobileNumber { get; init; }
    public string? SupervisorEmailAddress { get; init; }
    public string? SupervisorAddress { get; init; }

    public bool HasSupervisor => !string.IsNullOrWhiteSpace(SupervisorName)
        || !string.IsNullOrWhiteSpace(SupervisorTelephoneNumber)
        || !string.IsNullOrWhiteSpace(SupervisorMobileNumber)
        || !string.IsNullOrWhiteSpace(SupervisorEmailAddress)
        || !string.IsNullOrWhiteSpace(SupervisorAddress);

    public string FullName => string.Join(' ', new[] { FirstName, SecondName, LastName }
        .Where(s => !string.IsNullOrWhiteSpace(s)));

    internal static ParticipantAdvancedSearchDto From(CandidateDto c) => new()
    {
        Identifier = c.Identifier,
        FirstName = c.FirstName,
        SecondName = c.SecondName,
        LastName = c.LastName,
        DateOfBirth = c.DateOfBirth,
        Gender = c.Gender,
        NomisNumber = c.NomisNumber,
        Crn = c.Crn,
        PncNumber = c.PncNumber,
        PrimarySource = c.Primary,
        LocationDescription = c.LocationDescription,
        CustodyLocation = c.CustodyLocation,
        CommunityLocation = c.CommunityLocation,
        IsInCustody = c.IsInCustody,
        IsActive = c.IsActive,
        RegistrationDetails = c.RegistrationDetails
    };
}
