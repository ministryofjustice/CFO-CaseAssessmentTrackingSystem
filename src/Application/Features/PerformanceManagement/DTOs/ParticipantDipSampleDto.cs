
namespace Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

public class ParticipantDipSampleDto
{
    public required string ParticipantId { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required DateOnly DateOfBirth { get; init; }

    public required string? Nationality { get; init; }

    public required string? SupportWorker { get; init; }

    public required string EnrolmentLocation { get; init; }

    public required string CurrentLocation { get; init; }

    public DipEventInformation[] PertinentEvents { get; set;} = [];
    
    public DateOnly ConsentDate { get; set; }

    public required ComplianceAnswer CsoAnswer { get; set; }
    public string? CsoComments { get; set; }
    public DipSampleAnswer HasClearParticipantJourney { get; set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer ShowsTaskProgression { get; set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer ActivitiesLinkToTasks { get; set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer TTGDemonstratesGoodPRIProcess { get; set; } = DipSampleAnswer.NotAnswered;
    public DipSampleAnswer SupportsJourneyAndAlignsWithDoS { get; set; } = DipSampleAnswer.NotAnswered;
    

    public required ComplianceAnswer CpmAnswer { get; set; }
    public string? CpmComments { get; set; }
}