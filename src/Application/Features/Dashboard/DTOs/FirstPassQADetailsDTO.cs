namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class FirstPassQADetailsDto
{
    [Description("Participant Id")] 
    public required string ParticipantId { get; set; } 
    
    [Description("Participant")]
    public required string ParticipantName { get; set; } = default!;
    
    [Description("Queue")]
    public required string Queue { get; set; } 
   
    [Description("Last Modified")]
    public required DateTime? LastModified { get; set; } 
    
    [Description("QA User")]
    public required string QAUser { get; set; } 
    
    [Description("Is Accepted")]
    public required string IsAccepted { get; set; } 
    
    [Description("Escalated")]
    public required string Escalated { get; set; }
    
    [Description("Note")]
    public required string Note { get; set; }
}