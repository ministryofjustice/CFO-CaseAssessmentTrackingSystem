using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

public class PriSummaryDto
{
    public required string ParticipantId { get; set; }

    public required DateTime Created { get; set; }
    public required string CreatedBy { get; set; }
    public Guid ObjectiveId { get; private set; }
    public ObjectiveTaskDto[] ObjectiveTasks { get; set; } = [];
    public DateOnly? ActualReleaseDate { get; set; }       
    public required PriStatus Status { get; set; }
    public DateTime? CompletedOn { get; private set; }
    public string? CompletedBy { get; private set; }
    public PriAbandonReason? AbandonReason { get; private set; }

    //TTG warning 4 weeks from the actual release date i.e. 4 weeks * 7 days  = 28 days
    public DateOnly? TTGDueDate => ActualReleaseDate?.AddDays(28);

    public int? DaysUntilTTGDueDate => TTGDueDate.HasValue
                                        ? (int?)(TTGDueDate.Value.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow.Date).DayNumber)
                                        : null;

    public bool IsTTGTaskIncomplete => ObjectiveTasks.Length > 0
                                    &&  ObjectiveTasks.First(t => t.Index == 2).IsCompleted == false;
    public bool IsFirstTTGWarningApplicable => IsTTGTaskIncomplete 
                                            && DaysUntilTTGDueDate.HasValue 
                                            && DaysUntilTTGDueDate.Value > 7 
                                            && DaysUntilTTGDueDate.Value <= 14;
    public bool IsFinalTTGWarningApplicable => IsTTGTaskIncomplete 
                                            && DaysUntilTTGDueDate.HasValue 
                                            && DaysUntilTTGDueDate.Value <= 7;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PRI, PriSummaryDto>(MemberList.None);
        }
    }
}