using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class ParticipantDto
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    
    public EnrolmentStatus? EnrolmentStatus { get; set; }
    
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantDto>();

           
        }
    }
}