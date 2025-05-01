using MassTransit.Futures.Contracts;

namespace Cfo.Cats.Application.Features.Offloc.DTOs;

public record PersonalDetailsDto
{
    public required string NomsNumber { get; set; }

    [Description("First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Description("Second Name")]
    public string SecondName { get; set; } = string.Empty;

    [Description("Surname")]
    public string Surname { get; set; } = string.Empty;

    [Description("Date Of Birth")]
    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    [Description("Maternity Status")]
    public string MaternityStatus { get; set; } = string.Empty;


    public string Nationality { get; set; } = string.Empty;

    public string Religion { get; set; } = string.Empty;

    [Description("Marital Status")]
    public string MaritalStatus { get; set; } = string.Empty;

    [Description("Ethnicity")]
    public string EthnicGroup { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public MainOffence[] MainOffences { get; set; } = [];

    public OtherOffence[] OtherOffences { get; set; } = [];
}

public record MainOffence
{
    [Description("Main Offence")]
    public required string MainOffence1 { get; set; }
    [Description("Date Of First Conviction")]
    public DateOnly? DateFirstConviction { get; set; }
    public bool IsActive { get; set; }
}

public record OtherOffence
{
    public required string Details { get; set; }
}