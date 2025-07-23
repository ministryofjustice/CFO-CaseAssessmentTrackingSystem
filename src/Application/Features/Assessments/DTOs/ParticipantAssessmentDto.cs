using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class ParticipantAssessmentDto
{
    public required Guid Id { get; set; }
    public required string ParticipantId { get; set; } 
    public required PathwayScore[] PathwayScore { get; set; }
    public required DateTime CreatedDate { get; set; }
    public DateTime? Completed { get;  set; }
    public string? CompletedBy { get; set; }
    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ParticipantAssessment, ParticipantAssessmentDto>()
                .ForMember(p => p.Id, options => options.MapFrom(source => source.Id))
                .ForMember(p => p.CreatedDate, options => options.MapFrom(source => source.Created))
                .ForMember(p => p.PathwayScore, options => options.MapFrom(source => source.Scores))
                .ForMember(p => p.Completed, options => options.MapFrom(source => source.Completed))
                .ForMember(p => p.CompletedBy, options => options.MapFrom(source => source.CompletedBy))
                .ForMember(p => p.LocationId, options => options.MapFrom(source => source.LocationId))
                .ForMember(p => p.LocationName, options => options.Ignore());
        }
    }
}

