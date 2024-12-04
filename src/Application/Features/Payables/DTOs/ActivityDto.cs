using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Payables.DTOs;

public record class ActivityDto(ActivityDefinition Definition, LocationDto Location);



//public record class ActivityDto(string Description, ActivityType Type);