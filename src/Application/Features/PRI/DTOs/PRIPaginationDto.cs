﻿using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.PRI.DTOs;

[Description("PRIs")]
public class PRIPaginationDto
{
    [Description("PRI Id")]
    public required Guid Id { get; set; } = default!;

    [Description("Participant Id")]
    public string? ParticipantId { get; set; }

    [Description("Actual Date Of Release")]
    public DateOnly? ActualReleaseDate { get; set; }

    [Description("Expected Date Of Release")]
    public DateOnly? ExpectedReleaseDate { get; set; }

    [Description("Community Support Worker")]
    public string? AssignedTo { get; private set; }

    [Description("Custody Support Worker")]
    public string? CreatedBy { get; set; }

    public LocationDto? ExpectedReleaseRegion { get; set; }

    //public bool IsCompleted { get; }
    //public DateOnly MeetingAttendedOn { get; set; }
    //public bool MeetingAttendedInPerson { get; set; }
    //public string? MeetingNotAttendedInPersonJustification { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.PRIs.PRI, PRIPaginationDto>();                
        }
    }
}