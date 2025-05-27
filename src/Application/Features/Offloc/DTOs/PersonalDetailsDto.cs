namespace Cfo.Cats.Application.Features.Offloc.DTOs;

public class SentenceDataDto
{
    public string NomsNumber { get; set; } = default!;
    public IEnumerable<SentenceInformationDto> SentenceInformation { get; set; } = default!;
    public MainOffenceDto[] MainOffence { get; set; } = [];
    public OtherOffenceDto[] OtherOffences { get; set; } = [];
}

public class MainOffenceDto
{
    public string? MainOffence { get; set; }

    public DateOnly? DateFirstConviction { get; set; }

    public bool IsActive { get; set; }
}

public class OtherOffenceDto
{
    public string Details { get; set; } = null!;

    public bool IsActive { get; set; }
}

public class SentenceInformationDto
{
    public DateOnly? FirstSentenced { get; set; }

    public int? SentenceYears { get; set; }

    public int? SentenceMonths { get; set; }

    public int? SentenceDays { get; set; }

    public DateOnly? EarliestPossibleReleaseDate { get; set; }

    public DateOnly? DateOfRelease { get; set; }

    public string? Sed { get; set; }

    public string? Hdced { get; set; }

    public string? Hdcad { get; set; }

    public string? Ped { get; set; }

    public string? Crd { get; set; }

    public string? Npd { get; set; }

    public string? Led { get; set; }

    public DateOnly? Tused { get; set; }

    public bool IsActive { get; set; }

}