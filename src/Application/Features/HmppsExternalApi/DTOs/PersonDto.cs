namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Core person record, as returned by the HMPPS External API's GET /v2/persons/{hmppsId} endpoint.
/// </summary>
public class PersonDto
{
    public string? FirstName { get; init; }
    public string? MiddleNames { get; init; }
    public string? LastName { get; init; }
    public string? DateOfBirth { get; init; }
    public bool? Disability { get; init; }
    public bool? InterestToImmigration { get; init; }
    public CodeDescriptionDto? Title { get; init; }
    public CodeDescriptionDto? Sex { get; init; }
    public CodeDescriptionDto? SexualOrientation { get; init; }
    public CodeDescriptionDto? Religion { get; init; }
    public CodeDescriptionDto? Ethnicity { get; init; }
    public List<PersonAliasDto>? Aliases { get; init; }
    public List<PersonAddressDto>? Addresses { get; init; }
    public List<CodeDescriptionDto>? Nationalities { get; init; }
    public PersonIdentifiersDto? Identifiers { get; init; }
}

public class CodeDescriptionDto
{
    public string? Code { get; init; }
    public string? Description { get; init; }
}

public class PersonAliasDto
{
    public string? FirstName { get; init; }
    public string? MiddleNames { get; init; }
    public string? LastName { get; init; }
    public CodeDescriptionDto? Title { get; init; }
    public CodeDescriptionDto? Sex { get; init; }
    public List<PersonAddressDto>? Addresses { get; init; }
}

public class PersonAddressDto
{
    public string? CprAddressId { get; init; }
    public bool? NoFixedAbode { get; init; }
    public string? StartDate { get; init; }
    public string? StartDateTime { get; init; }
    public string? EndDate { get; init; }
    public string? EndDateTime { get; init; }
    public string? Postcode { get; init; }
    public string? SubBuildingName { get; init; }
    public string? BuildingName { get; init; }
    public string? BuildingNumber { get; init; }
    public string? ThoroughfareName { get; init; }
    public string? DependentLocality { get; init; }
    public string? PostTown { get; init; }
    public string? County { get; init; }
    public string? Country { get; init; }
    public string? CountryCode { get; init; }
    public string? Uprn { get; init; }
    public CodeDescriptionDto? Status { get; init; }
    public string? Comment { get; init; }
    public bool? TypeVerified { get; init; }
    public List<AddressUsageDto>? Usages { get; init; }
    public List<AddressContactDto>? Contacts { get; init; }
}

public class AddressUsageDto
{
    public string? Code { get; init; }
    public string? Description { get; init; }
    public bool? IsActive { get; init; }
}

public class AddressContactDto
{
    public CodeDescriptionDto? Type { get; init; }
    public string? Value { get; init; }
    public string? Extension { get; init; }
}

public class PersonIdentifiersDto
{
    public List<string>? Crns { get; init; }
    public List<string>? PrisonNumbers { get; init; }
    public List<string>? DefendantIds { get; init; }
    public List<string>? Cids { get; init; }
    public List<string>? Pncs { get; init; }
    public List<string>? Cros { get; init; }
    public List<string>? NationalInsuranceNumbers { get; init; }
    public List<string>? DriverLicenseNumbers { get; init; }
    public List<string>? ArrestSummonsNumbers { get; init; }
}
