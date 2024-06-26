using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Candidates.DTOs;

public class CandidateDto
{
    /// <summary>
    /// The CATS identifier
    /// </summary>
    public string Identifier { get; set; }
    
    /// <summary>
    /// The first name of the candidate
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// The candidates last name
    /// </summary>
    public string LastName { get; set; } 
    
    /// <summary>
    /// The candidates date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// A collection of identifiers from external systems.
    /// </summary>
    public string[] ExternalIdentifiers { get; set; } = [];


    /// <summary>
    /// Indicates whether the candidate is marked as active in the data source(s).
    /// </summary>
    public bool IsActive { get; set; }

    public string Gender { get; set; }
    
    /// <summary>
    /// The location CATS thinks the user is registered at
    /// </summary>
    public string CurrentLocation { get; set; }

    public EnrolmentStatus? EnrolmentStatus { get; set; }
    
    public string? ReferralSource { get; set; }
}
