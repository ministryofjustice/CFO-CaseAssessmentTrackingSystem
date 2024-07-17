using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

/// <summary>
/// Represents the initial dashboard 
/// </summary>
public class ParticipantSummaryDto
{
    
    public required string Id { get; set; }
    
    /// <summary>
    /// The full name of the participant
    /// </summary>
    public required string ParticipantName { get; set; }
    
    /// <summary>
    /// The current location of the participant
    /// </summary>
    public required string Location { get; set; }
    
    /// <summary>
    /// The participant's date of birth
    /// </summary>
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    ///  The current enrolment status of the participant
    /// </summary>
    public EnrolmentStatus EnrolmentStatus { get; set; } = EnrolmentStatus.PendingStatus;
    
    /// <summary>
    /// The person who "owns" this participant's case. Usually the support worker.
    /// </summary>
    public required string OwnerName { get; set; }

    public AssessmentSummaryDto[] Assessments { get; set; } = [];


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Participant, ParticipantSummaryDto>(MemberList.None)
                .ForMember(target => target.Id, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.Location, options => options.MapFrom(source => source.CurrentLocation.Name))
                .ForMember(target => target.OwnerName, options => options.MapFrom(source => source.Owner!.DisplayName))
                .ForMember(target => target.ParticipantName, options => options.MapFrom(source => source.FirstName + ' ' + source.LastName));

            CreateMap<ParticipantAssessment, AssessmentSummaryDto>()
                .ForMember(target => target.AssessmentId, options => options.MapFrom(source => source.Id))
                .ForMember(target => target.AssessmentDate, options => options.MapFrom(source => source.Created))
                .ForMember(target => target.AssessmentCreator, options => options.MapFrom(source => source.CreatedBy))
                .ForMember(target => target.AssessmentScored, options => options.MapFrom(source => source.Scores.All(s => s.Score >= 0)));
        }
    }


}

public class AssessmentSummaryDto
{
    /// <summary>
    /// The id of the latest assessment
    /// </summary>
    public Guid? AssessmentId { get; set; }
    
    
    /// <summary>
    /// If there are any assessments these are the dates they latest one was created.
    /// </summary>
    public DateTime? AssessmentDate { get; set; }
    
    /// <summary>
    /// Who created the most recent assessment (if available)
    /// </summary>
    public string? AssessmentCreator { get; set; }
    
    /// <summary>
    /// Has the latest assessment been scored? This can be a surrogate for
    /// submitted and should make the assessment read-only
    /// </summary>
    public bool? AssessmentScored { get; set; }
}
