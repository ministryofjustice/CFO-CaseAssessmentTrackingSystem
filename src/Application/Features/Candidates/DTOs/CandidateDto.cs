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
    /// The second (or middle) name of the candidate
    /// </summary>
    public string SecondName { get; set; }
    
    /// <summary>
    /// The candidates last name
    /// </summary>
    public string LastName { get; set; } 
    
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
    /// Indicates whether the primary record is marked as active in the data source(s).
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The candidates gender (Male/Female).
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// The candidates nationality
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// The candidates ethnicity
    /// </summary>
    public string Ethnicity { get; set; }

    /// <summary>
    /// The primary source of information (NOMIS/DELIUS).
    /// </summary>
    public string Origin { get; set; }
    
    /// <summary>
    /// The location CATS thinks the user is registered at
    /// </summary>
    public string CurrentLocation { get; set; }

    public EnrolmentStatus? EnrolmentStatus { get; set; }
    
    public string? ReferralSource { get; set; }
}
