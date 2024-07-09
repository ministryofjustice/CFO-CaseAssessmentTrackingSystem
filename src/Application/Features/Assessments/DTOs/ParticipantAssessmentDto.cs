using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class ParticipantAssessmentDto
{
    public required string ParticipantId { get; set; } 
    public required PathwayScore[] PathwayScore { get; set; }
    public required DateTime CreatedDate { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ParticipantAssessment, ParticipantAssessmentDto>()
                .ForMember(p => p.CreatedDate, options => options.MapFrom(source => source.Created))
                .ForMember(p => p.PathwayScore, options => options.MapFrom(source => source.Scores));
        }
    }
}

