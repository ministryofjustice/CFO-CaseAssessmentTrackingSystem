using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Mappings;

#pragma warning disable CS8601, CS8602 // Possible null reference assignment, Dereference of a possibly null reference.
public static class ActivityMappings
{
    public static readonly Expression<Func<Activity, ActivitySummaryDto>> ToSummaryDto =
        entity => new()
        {
            Id = entity.Id,
            TookPlaceAtLocationName = entity.TookPlaceAtLocation.Name,
            TaskId = entity.TaskId,
            Created = entity.Created!.Value,
            CommencedOn = entity.CommencedOn,
            Definition = entity.Definition,
            Status = entity.Status,
            Expiry = entity.Expiry,
            AdditionalInformation = entity.AdditionalInformation
        };
}
#pragma warning restore CS8601, CS8602 // Possible null reference assignment, Dereference of a possibly null reference.