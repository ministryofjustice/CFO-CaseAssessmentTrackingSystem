using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;

[RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
public class Command : IRequest<Result>
{
    public required UserProfile CurrentUser { get; set; }
    
    public required string ParticipantId { get; set; }
    public required Guid DipSampleId { get; set; }

    [Description("Does the pathway plan, thematic objectives and tasks (where applicable) show a clear participant story/journey?")]
    public DipSampleAnswer HasClearParticipantJourney { get; set; } = DipSampleAnswer.NotAnswered;

    [Description("Is there progression shown against the tasks?")]
    public DipSampleAnswer ShowsTaskProgression { get; set; } = DipSampleAnswer.NotAnswered;

    [Description("Do activities link to the tasks?")]
    public DipSampleAnswer ActivitiesLinkToTask { get; set; }= DipSampleAnswer.NotAnswered;

    [Description("If applicable, does the TTG objective and commencement of work against the tasks show good demonstration of the PRI process?")]
    public DipSampleAnswer TTGDemonstratesGoodPRIProcess { get; set; } = DipSampleAnswer.NotAnswered;

    [Description("If applicable, do Human Citizenship, Community Social and Intervention Services link to the participant story/journey, do they link to the DoS and do they demonstrate good quality outcomes including VFM?")]
    public DipSampleAnswer SupportsJourneyAndAlignsWithDoS { get; set; } = DipSampleAnswer.NotAnswered;
   
    [Description("If applicable are Employment, ETE and ISWS templates in line with the REG?")] 
    public DipSampleAnswer TemplatesAlignWithREG { get; set; }= DipSampleAnswer.NotAnswered;

    [Description("Comments")]
    public string? Comments { get; set; }

    [Description("Does this review meet compliance")]
    public ComplianceAnswer ComplianceAnswer { get; set; }= ComplianceAnswer.NotAnswered;
    
}