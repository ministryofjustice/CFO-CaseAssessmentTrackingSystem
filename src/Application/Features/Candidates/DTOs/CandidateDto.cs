using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Entities.Participants;

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
    public DateTime? DateOfBirth { get; set; }
    
    /// <summary>
    /// A collection of identifiers from external systems.
    /// </summary>
    public string[] ExternalIdentifiers { get; set; }
    
    /// <summary>
    /// The location CATS thinks the user is registered at
    /// </summary>
    public string CurrentLocation { get; set; }

    public EnrolmentStatus? EnrolmentStatus { get; set; }
    
    public string? ReferralSource { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Candidate, CandidateDto>(MemberList.None)
                .ForMember(candidateDto => candidateDto.Identifier, 
                    options => options.MapFrom(candidate => candidate.Id))
                .ForMember(candidateDto => candidateDto.FirstName, 
                    options => options.MapFrom(candidate => candidate.FirstName))
                .ForMember(candidateDto => candidateDto.LastName, 
                    options => options.MapFrom(candidate => candidate.LastName))
                .ForMember(candidateDto => candidateDto.CurrentLocation,
                    options => options.MapFrom(candidate => candidate.CurrentLocation.Name))
                .ForMember(candidateDto => candidateDto.ExternalIdentifiers, 
                    options => options.MapFrom(candidate => candidate.Identifiers.Select(p => p.IdentifierValue).ToArray()));
                
        }
    }
}
