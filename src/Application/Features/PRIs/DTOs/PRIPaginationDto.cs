using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.PRIs.DTOs;

[Description("PRIs")]
public class PRIPaginationDto
{
    [Description("PRI Id")]
    public required Guid Id { get; set; } = default!;

    [Description("Participant Id")]
    public required string ParticipantId { get; set; }

    [Description("Participant Name")]
    public required string ParticipantName { get; set; }

    [Description("First Name")]
    public required string FirstName { get; set; }

    [Description("Last Name")]
    public required string LastName { get; set; }

    [Description("Actual Date Of Release")]
    public DateOnly? ActualReleaseDate { get; set; }

    [Description("Expected Date Of Release")]
    public DateOnly? ExpectedReleaseDate { get; set; }

    [Description("Community Support Worker")]
    public string? AssignedTo { get; private set; }

    [Description("Custody Support Worker")]
    public string? CreatedBy { get; set; }

    public LocationDto? ExpectedReleaseRegion { get; set; }

    [Description("Expected Release Region")]
    public string? ExpectedReleaseRegionName { get; set; }

    public required bool ParticipantIsActive { get; set; }

    private class Mapping : Profile
    {
        public Mapping() =>
            CreateMap<Domain.Entities.PRIs.PRI, PRIPaginationDto>()
                .ForMember(dest => dest.ParticipantName, opt => opt.MapFrom(src => src.Participant!.FullName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Participant!.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Participant!.LastName))
                .ForMember(dest => dest.ExpectedReleaseRegionName, opt => opt.MapFrom(src => src.ExpectedReleaseRegion != null ? src.ExpectedReleaseRegion.Name : null))
                .ForMember(dest => dest.ParticipantIsActive, opt => opt.MapFrom(src => src.Participant!.IsActive()));
    }
}