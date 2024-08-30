using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Candidates.DTOs;

public class CandidateDto
{
    /// <summary>
    /// The CATS identifier
    /// </summary>
    public required string Identifier { get; set; }
    
    /// <summary>
    /// The first name of the candidate
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The second (or middle) name of the candidate
    /// </summary>
    public string? SecondName { get; set; }
    
    /// <summary>
    /// The candidates last name
    /// </summary>
    public required string LastName { get; set; } 
    
    /// <summary>
    /// The candidates date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// The candidates NOMIS Number (if applicable).
    /// </summary>
    public string? NomisNumber { get; set; }

    /// <summary>
    /// The candidates Crn (if applicable).
    /// </summary>
    public string? Crn { get; set; }

    /// <summary>
    /// The candidates PNC Number (if applicable).
    /// </summary>
    public string? PncNumber { get; set; }

    /// <summary>
    /// Indicates whether the primary record is marked as active in the data source(s).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The candidates gender (Male/Female).
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// The candidates nationality
    /// </summary>
    public required string Nationality { get; set; }

    /// <summary>
    /// The candidates ethnicity
    /// </summary>
    public required string Ethnicity { get; set; }

    /// <summary>
    /// The primary source of information (NOMIS/DELIUS).
    /// </summary>
    public required string Primary { get; set; }

    public string? EstCode { get; set; }
    public string? OrgCode { get; set; }

    public EnrolmentStatus? EnrolmentStatus { get; set; }
    
    public string? ReferralSource { get; set; }

    public string? LocationDescription { get; set; }

    public int MappedLocationId { get; set; } = 0;
}
