﻿using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Payables.Specifications;

public class ActivitiesAdvancedFilter : PaginationFilter
{
    public required string ParticipantId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? ObjectiveId { get; set; }
    public DateTime? CompletedStart { get; set; }
    public DateTime? CompletedEnd { get; set; }
    public LocationDto? Location { get; set; }
    public List<ActivityType>? IncludeTypes { get; set; }
}

public enum ActivitiesListView
{
    [Description("Default")] Default = 0,
}