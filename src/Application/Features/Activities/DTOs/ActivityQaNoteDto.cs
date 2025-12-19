using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.DTOs;
#nullable disable

public class ActivityQaNoteDto
{
    public required DateTime Created { get; set; }
    public required string Message { get; set; }
    public required string CreatedBy { get; set; }
    public required string TenantName { get; set; }
    public required bool IsExternal { get; set; }
    
    public bool IsExpanded { get; set; }
    
    public static ActivityQaNoteDto FromEntity(ActivityQueueEntryNote entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (entity.CreatedByUser is null)
        {
            throw new InvalidOperationException("CreatedByUser cannot be null when mapping ActivityQueueEntryNote.");
        }
        
        return new ActivityQaNoteDto
        {
            Created = entity.Created ?? DateTime.MinValue,
            Message = entity.Message,
            CreatedBy = entity?.CreatedByUser?.DisplayName ?? string.Empty,
            TenantName = entity?.CreatedByUser?.TenantName ?? string.Empty,
            IsExternal = entity.IsExternal,
            IsExpanded = false
        };
    }
}