using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Matching.Core.Search;

namespace Cfo.Cats.Infrastructure.Services.Candidates;

public class DummyCandidateService(IUnitOfWork unitOfWork) : ICandidateService
{
    public IReadOnlyList<CandidateDto> Candidates =>
    [
        new CandidateDto
        {
            Identifier = "1CFG1109X",
            FirstName = "Barry",
            LastName = "Stone",
            DateOfBirth = new DateTime(1979, 1, 18),
            Gender = "Male",
            Crn = "B008622",
            NomisNumber = "A7022BA",
            Primary = "DELIUS",
            OrgCode = "LNS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3305M",
            FirstName = "Clark",
            LastName = "Queen",
            DateOfBirth = new DateTime(1985, 7, 16),
            Gender = "Female",
            Crn = "B004786",
            NomisNumber = "A7373XA",
            Primary = "NOMIS",
            EstCode = "HBI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3647Z",
            FirstName = "Hal",
            LastName = "Parker",
            DateOfBirth = new DateTime(2000, 8, 31),
            Gender = "Male",
            Crn = "B006754",
            NomisNumber = "A1118OA",
            Primary = "NOMIS",
            EstCode = "N22",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4819M",
            FirstName = "Arthur",
            LastName = "Stone",
            DateOfBirth = new DateTime(1961, 3, 8),
            Gender = "Female",
            Crn = "B008866",
            NomisNumber = "A6972XA",
            Primary = "NOMIS",
            EstCode = "GNI",
            Nationality = "",
            Ethnicity = "",
            IsActive = false
        },
        new CandidateDto
        {
            Identifier = "1CFG3977A",
            FirstName = "Oliver",
            LastName = "Kent",
            DateOfBirth = new DateTime(1966, 8, 16),
            Gender = "Male",
            Crn = "B004529",
            NomisNumber = "A3749HA",
            Primary = "NOMIS",
            EstCode = "SKI",
            Nationality = "",
            Ethnicity = "",
            IsActive = false
        },
        new CandidateDto
        {
            Identifier = "1CFG4580T",
            FirstName = "Barry",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1979, 6, 18),
            Gender = "Male",
            Crn = "B007498",
            NomisNumber = "A6102NA",
            Primary = "NOMIS",
            EstCode = "N07",
            Nationality = "",
            Ethnicity = "",
            IsActive = false
        },
        new CandidateDto
        {
            Identifier = "1CFG7754K",
            FirstName = "Victor",
            LastName = "Kent",
            DateOfBirth = new DateTime(1956, 10, 21),
            Gender = "Male",
            Crn = "B003913",
            NomisNumber = "A2817BA",
            Primary = "NOMIS",
            EstCode = "FYI",
            Nationality = "",
            Ethnicity = "",
            IsActive = false
        },
        new CandidateDto
        {
            Identifier = "1CFG1873Q",
            FirstName = "Arthur",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1988, 8, 15),
            Gender = "Male",
            Crn = "B004044",
            NomisNumber = "A8654XA",
            Primary = "NOMIS",
            EstCode = "YSN",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5221C",
            FirstName = "Victor",
            LastName = "Parker",
            DateOfBirth = new DateTime(1977, 9, 14),
            Gender = "Male",
            Crn = "B003506",
            NomisNumber = "A3576SA",
            Primary = "NOMIS",
            EstCode = "OUT",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5006P",
            FirstName = "Clark",
            LastName = "Kent",
            DateOfBirth = new DateTime(1988, 9, 9),
            Gender = "Female",
            Crn = "B001407",
            NomisNumber = "A8065EA",
            Primary = "NOMIS",
            EstCode = "CWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2001T",
            FirstName = "Oliver",
            LastName = "Jordan",
            DateOfBirth = new DateTime(2004, 9, 30),
            Gender = "Female",
            Crn = "B003237",
            NomisNumber = "A6587YA",
            Primary = "NOMIS",
            EstCode = "N31",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5437L",
            FirstName = "Hal",
            LastName = "Parker",
            DateOfBirth = new DateTime(1956, 9, 25),
            Gender = "Male",
            Crn = "B003171",
            NomisNumber = "A6952ZA",
            Primary = "NOMIS",
            EstCode = "N26",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2883L",
            FirstName = "Oliver",
            LastName = "Prince",
            DateOfBirth = new DateTime(1961, 8, 29),
            Gender = "Male",
            Crn = "B007887",
            NomisNumber = "A3402IA",
            Primary = "NOMIS",
            EstCode = "GMI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2167R",
            FirstName = "Bruce",
            LastName = "Allen",
            DateOfBirth = new DateTime(1969, 8, 27),
            Gender = "Female",
            Crn = "B009234",
            NomisNumber = "A7364GA",
            Primary = "NOMIS",
            EstCode = "PBI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9064R",
            FirstName = "Bruce",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1982, 7, 19),
            Gender = "Male",
            Crn = "B004752",
            NomisNumber = "A6222SA",
            Primary = "NOMIS",
            EstCode = "N07",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1900I",
            FirstName = "Oliver",
            LastName = "Curry",
            DateOfBirth = new DateTime(1983, 6, 30),
            Gender = "Male",
            Crn = "B005270",
            NomisNumber = "A9293CA",
            Primary = "NOMIS",
            EstCode = "IWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2847D",
            FirstName = "Bruce",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1991, 9, 17),
            Gender = "Female",
            Crn = "B005459",
            NomisNumber = "A5884JA",
            Primary = "NOMIS",
            EstCode = "DTV",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7892B",
            FirstName = "Arthur",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1952, 8, 22),
            Gender = "Male",
            Crn = "B006933",
            NomisNumber = "A9162QA",
            Primary = "NOMIS",
            EstCode = "C19",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6906H",
            FirstName = "Diana",
            LastName = "Curry",
            DateOfBirth = new DateTime(1980, 8, 9),
            Gender = "Male",
            Crn = "B005260",
            NomisNumber = "A6150IA",
            Primary = "NOMIS",
            EstCode = "EHI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1333S",
            FirstName = "Peter",
            LastName = "Allen",
            DateOfBirth = new DateTime(1989, 12, 2),
            Gender = "Female",
            Crn = "B002552",
            NomisNumber = "A2257FA",
            Primary = "NOMIS",
            EstCode = "BXI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1479H",
            FirstName = "Peter",
            LastName = "Prince",
            DateOfBirth = new DateTime(1956, 7, 18),
            Gender = "Female",
            Crn = "B004442",
            NomisNumber = "A6948JA",
            Primary = "NOMIS",
            EstCode = "LTS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7788V",
            FirstName = "Diana",
            LastName = "Lance",
            DateOfBirth = new DateTime(1953, 10, 27),
            Gender = "Male",
            Crn = "B004219",
            NomisNumber = "A9649AA",
            Primary = "NOMIS",
            EstCode = "ESX",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1553F",
            FirstName = "Victor",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1978, 12, 8),
            Gender = "Female",
            Crn = "B008576",
            NomisNumber = "A9860RA",
            Primary = "NOMIS",
            EstCode = "C20",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4867Q",
            FirstName = "Oliver",
            LastName = "Jordan",
            DateOfBirth = new DateTime(2000, 2, 3),
            Gender = "Male",
            Crn = "B008340",
            NomisNumber = "A1567LA",
            Primary = "NOMIS",
            EstCode = "CWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5854G",
            FirstName = "Dinah",
            LastName = "Queen",
            DateOfBirth = new DateTime(1952, 10, 9),
            Gender = "Female",
            Crn = "B009885",
            NomisNumber = "A9937FA",
            Primary = "NOMIS",
            EstCode = "RCI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5469L",
            FirstName = "Peter",
            LastName = "Queen",
            DateOfBirth = new DateTime(1953, 11, 18),
            Gender = "Male",
            Crn = "B001257",
            NomisNumber = "A4692DA",
            Primary = "NOMIS",
            EstCode = "WCI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5502H",
            FirstName = "Bruce",
            LastName = "Kent",
            DateOfBirth = new DateTime(1978, 2, 11),
            Gender = "Male",
            Crn = "B009194",
            NomisNumber = "A1411YA",
            Primary = "NOMIS",
            EstCode = "WMI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2825E",
            FirstName = "Peter",
            LastName = "Prince",
            DateOfBirth = new DateTime(1975, 12, 23),
            Gender = "Female",
            Crn = "B009268",
            NomisNumber = "A7907DA",
            Primary = "NOMIS",
            EstCode = "ESX",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8516T",
            FirstName = "Dinah",
            LastName = "Kent",
            DateOfBirth = new DateTime(1954, 6, 19),
            Gender = "Female",
            Crn = "B009761",
            NomisNumber = "A7400NA",
            Primary = "NOMIS",
            EstCode = "SDI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5298O",
            FirstName = "Victor",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1961, 7, 28),
            Gender = "Male",
            Crn = "B007256",
            NomisNumber = "A8430TA",
            Primary = "NOMIS",
            EstCode = "AYI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9775P",
            FirstName = "Barry",
            LastName = "Stone",
            DateOfBirth = new DateTime(1960, 8, 14),
            Gender = "Female",
            Crn = "B008155",
            NomisNumber = "A1067SA",
            Primary = "NOMIS",
            EstCode = "N06",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6849L",
            FirstName = "Bruce",
            LastName = "Parker",
            DateOfBirth = new DateTime(1977, 2, 20),
            Gender = "Male",
            Crn = "B009597",
            NomisNumber = "A7483AA",
            Primary = "NOMIS",
            EstCode = "WWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8347B",
            FirstName = "Clark",
            LastName = "Queen",
            DateOfBirth = new DateTime(2005, 10, 6),
            Gender = "Female",
            Crn = "B005305",
            NomisNumber = "A1056JA",
            Primary = "NOMIS",
            EstCode = "NWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4189E",
            FirstName = "Bruce",
            LastName = "Curry",
            DateOfBirth = new DateTime(1956, 4, 14),
            Gender = "Female",
            Crn = "B006480",
            NomisNumber = "A3064CA",
            Primary = "NOMIS",
            EstCode = "DTV",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5514M",
            FirstName = "Clark",
            LastName = "Curry",
            DateOfBirth = new DateTime(1962, 5, 28),
            Gender = "Female",
            Crn = "B005528",
            NomisNumber = "A7236EA",
            Primary = "NOMIS",
            EstCode = "FHI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5252T",
            FirstName = "Peter",
            LastName = "Queen",
            DateOfBirth = new DateTime(1964, 9, 23),
            Gender = "Female",
            Crn = "B008719",
            NomisNumber = "A4570WA",
            Primary = "NOMIS",
            EstCode = "C08",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6789D",
            FirstName = "Clark",
            LastName = "Parker",
            DateOfBirth = new DateTime(1968, 10, 24),
            Gender = "Male",
            Crn = "B002584",
            NomisNumber = "A6440GA",
            Primary = "NOMIS",
            EstCode = "SLI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1848U",
            FirstName = "Barry",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1981, 6, 9),
            Gender = "Female",
            Crn = "B004852",
            NomisNumber = "A7126QA",
            Primary = "NOMIS",
            EstCode = "BWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4892S",
            FirstName = "Arthur",
            LastName = "Allen",
            DateOfBirth = new DateTime(1960, 10, 9),
            Gender = "Male",
            Crn = "B008270",
            NomisNumber = "A6249NA",
            Primary = "NOMIS",
            EstCode = "MDI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5866O",
            FirstName = "Peter",
            LastName = "Jordan",
            DateOfBirth = new DateTime(2001, 10, 7),
            Gender = "Female",
            Crn = "B008561",
            NomisNumber = "A9186BA",
            Primary = "NOMIS",
            EstCode = "DRS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7842D",
            FirstName = "Diana",
            LastName = "Queen",
            DateOfBirth = new DateTime(1982, 7, 15),
            Gender = "Female",
            Crn = "B005033",
            NomisNumber = "A2699ZA",
            Primary = "NOMIS",
            EstCode = "WTI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8262O",
            FirstName = "Peter",
            LastName = "Curry",
            DateOfBirth = new DateTime(1984, 7, 2),
            Gender = "Male",
            Crn = "B001022",
            NomisNumber = "A2865UA",
            Primary = "NOMIS",
            EstCode = "EXI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6353O",
            FirstName = "Hal",
            LastName = "Stone",
            DateOfBirth = new DateTime(1952, 1, 15),
            Gender = "Male",
            Crn = "B002433",
            NomisNumber = "A4973LA",
            Primary = "NOMIS",
            EstCode = "N52",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2308N",
            FirstName = "Clark",
            LastName = "Parker",
            DateOfBirth = new DateTime(2004, 6, 26),
            Gender = "Male",
            Crn = "B006501",
            NomisNumber = "A3961UA",
            Primary = "NOMIS",
            EstCode = "SPI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8867D",
            FirstName = "Arthur",
            LastName = "Curry",
            DateOfBirth = new DateTime(1951, 9, 20),
            Gender = "Male",
            Crn = "B009689",
            NomisNumber = "A8052RA",
            Primary = "NOMIS",
            EstCode = "DWI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5131C",
            FirstName = "Barry",
            LastName = "Allen",
            DateOfBirth = new DateTime(1981, 6, 26),
            Gender = "Male",
            Crn = "B005370",
            NomisNumber = "A5295TA",
            Primary = "NOMIS",
            EstCode = "PFI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8864M",
            FirstName = "Arthur",
            LastName = "Curry",
            DateOfBirth = new DateTime(1985, 3, 14),
            Gender = "Female",
            Crn = "B001879",
            NomisNumber = "A4162OA",
            Primary = "NOMIS",
            EstCode = "PDI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5544Z",
            FirstName = "Hal",
            LastName = "Allen",
            DateOfBirth = new DateTime(1954, 8, 11),
            Gender = "Female",
            Crn = "B007141",
            NomisNumber = "A3667GA",
            Primary = "NOMIS",
            EstCode = "CHS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9475S",
            FirstName = "Victor",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1950, 4, 10),
            Gender = "Female",
            Crn = "B009820",
            NomisNumber = "A1011FA",
            Primary = "NOMIS",
            EstCode = "BMI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8570M",
            FirstName = "Bruce",
            LastName = "Queen",
            DateOfBirth = new DateTime(1951, 7, 21),
            Gender = "Male",
            Crn = "B005022",
            NomisNumber = "A2364MA",
            Primary = "NOMIS",
            EstCode = "TRN",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1646R",
            FirstName = "Hal",
            LastName = "Kent",
            DateOfBirth = new DateTime(1957, 8, 21),
            Gender = "Female",
            Crn = "B008390",
            NomisNumber = "A9321HA",
            Primary = "NOMIS",
            EstCode = "FEI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7199D",
            FirstName = "Hal",
            LastName = "Prince",
            DateOfBirth = new DateTime(1990, 6, 25),
            Gender = "Female",
            Crn = "B006153",
            NomisNumber = "A2303FA",
            Primary = "NOMIS",
            EstCode = "MTI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8089M",
            FirstName = "Barry",
            LastName = "Parker",
            DateOfBirth = new DateTime(1982, 10, 31),
            Gender = "Female",
            Crn = "B005799",
            NomisNumber = "A8614GA",
            Primary = "NOMIS",
            EstCode = "HRI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5547B",
            FirstName = "Barry",
            LastName = "Lance",
            DateOfBirth = new DateTime(1990, 1, 17),
            Gender = "Female",
            Crn = "B002156",
            NomisNumber = "A7019IA",
            Primary = "NOMIS",
            EstCode = "FSI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2192B",
            FirstName = "Victor",
            LastName = "Allen",
            DateOfBirth = new DateTime(1999, 7, 30),
            Gender = "Female",
            Crn = "B009460",
            NomisNumber = "A2301TA",
            Primary = "NOMIS",
            EstCode = "GHI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9918N",
            FirstName = "Barry",
            LastName = "Stone",
            DateOfBirth = new DateTime(1962, 3, 25),
            Gender = "Male",
            Crn = "B007272",
            NomisNumber = "A7368NA",
            Primary = "NOMIS",
            EstCode = "STI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1461R",
            FirstName = "Peter",
            LastName = "Parker",
            DateOfBirth = new DateTime(1995, 3, 25),
            Gender = "Male",
            Crn = "B004903",
            NomisNumber = "A9368JA",
            Primary = "NOMIS",
            EstCode = "WNI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2867U",
            FirstName = "Diana",
            LastName = "Lance",
            DateOfBirth = new DateTime(1968, 6, 27),
            Gender = "Male",
            Crn = "B009542",
            NomisNumber = "A4368JA",
            Primary = "NOMIS",
            EstCode = "STI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3069P",
            FirstName = "Dinah",
            LastName = "Stone",
            DateOfBirth = new DateTime(1987, 6, 22),
            Gender = "Female",
            Crn = "B009809",
            NomisNumber = "A1436VA",
            Primary = "NOMIS",
            EstCode = "CDI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7072K",
            FirstName = "Arthur",
            LastName = "Queen",
            DateOfBirth = new DateTime(1975, 10, 5),
            Gender = "Male",
            Crn = "B009199",
            NomisNumber = "A6247XA",
            Primary = "NOMIS",
            EstCode = "C04",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2438I",
            FirstName = "Barry",
            LastName = "Prince",
            DateOfBirth = new DateTime(1971, 7, 8),
            Gender = "Female",
            Crn = "B003456",
            NomisNumber = "A5176NA",
            Primary = "NOMIS",
            EstCode = "GCS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6580Z",
            FirstName = "Dinah",
            LastName = "Allen",
            DateOfBirth = new DateTime(1960, 3, 5),
            Gender = "Female",
            Crn = "B003215",
            NomisNumber = "A4206SA",
            Primary = "NOMIS",
            EstCode = "TCI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1303N",
            FirstName = "Diana",
            LastName = "Lance",
            DateOfBirth = new DateTime(1984, 5, 5),
            Gender = "Male",
            Crn = "B002668",
            NomisNumber = "A5319AA",
            Primary = "NOMIS",
            EstCode = "C20",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2282K",
            FirstName = "Barry",
            LastName = "Lance",
            DateOfBirth = new DateTime(1960, 11, 30),
            Gender = "Female",
            Crn = "B006078",
            NomisNumber = "A8725WA",
            Primary = "NOMIS",
            EstCode = "PVI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7502D",
            FirstName = "Hal",
            LastName = "Prince",
            DateOfBirth = new DateTime(1965, 9, 3),
            Gender = "Male",
            Crn = "B005846",
            NomisNumber = "A7652NA",
            Primary = "NOMIS",
            EstCode = "N23",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8236D",
            FirstName = "Bruce",
            LastName = "Allen",
            DateOfBirth = new DateTime(1999, 2, 24),
            Gender = "Male",
            Crn = "B001005",
            NomisNumber = "A6250UA",
            Primary = "NOMIS",
            EstCode = "OUT",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4926U",
            FirstName = "Peter",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1990, 4, 4),
            Gender = "Female",
            Crn = "B008641",
            NomisNumber = "A3877TA",
            Primary = "NOMIS",
            EstCode = "ESI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3204I",
            FirstName = "Bruce",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1958, 5, 2),
            Gender = "Male",
            Crn = "B007677",
            NomisNumber = "A1403DA",
            Primary = "NOMIS",
            EstCode = "WSI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3962X",
            FirstName = "Arthur",
            LastName = "Parker",
            DateOfBirth = new DateTime(1981, 2, 12),
            Gender = "Female",
            Crn = "B007012",
            NomisNumber = "A6308NA",
            Primary = "NOMIS",
            EstCode = "EXI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7281E",
            FirstName = "Peter",
            LastName = "Stone",
            DateOfBirth = new DateTime(1958, 7, 2),
            Gender = "Male",
            Crn = "B001642",
            NomisNumber = "A6119BA",
            Primary = "NOMIS",
            EstCode = "C07",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1626P",
            FirstName = "Bruce",
            LastName = "Curry",
            DateOfBirth = new DateTime(1996, 5, 24),
            Gender = "Male",
            Crn = "B002835",
            NomisNumber = "A5610CA",
            Primary = "NOMIS",
            EstCode = "TCI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1773W",
            FirstName = "Peter",
            LastName = "Allen",
            DateOfBirth = new DateTime(2005, 7, 21),
            Gender = "Male",
            Crn = "B007895",
            NomisNumber = "A9218RA",
            Primary = "NOMIS",
            EstCode = "EXI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG1359X",
            FirstName = "Bruce",
            LastName = "Parker",
            DateOfBirth = new DateTime(1954, 2, 23),
            Gender = "Male",
            Crn = "B004099",
            NomisNumber = "A5852KA",
            Primary = "NOMIS",
            EstCode = "WII",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5462D",
            FirstName = "Oliver",
            LastName = "Queen",
            DateOfBirth = new DateTime(1975, 7, 10),
            Gender = "Female",
            Crn = "B007065",
            NomisNumber = "A6399OA",
            Primary = "NOMIS",
            EstCode = "FYI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG5813Z",
            FirstName = "Hal",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1969, 12, 28),
            Gender = "Male",
            Crn = "B004062",
            NomisNumber = "A7138MA",
            Primary = "NOMIS",
            EstCode = "TSI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2975B",
            FirstName = "Peter",
            LastName = "Queen",
            DateOfBirth = new DateTime(1996, 2, 13),
            Gender = "Male",
            Crn = "B009564",
            NomisNumber = "A3824OA",
            Primary = "NOMIS",
            EstCode = "N01",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7191X",
            FirstName = "Dinah",
            LastName = "Parker",
            DateOfBirth = new DateTime(1992, 6, 27),
            Gender = "Male",
            Crn = "B002061",
            NomisNumber = "A4383XA",
            Primary = "NOMIS",
            EstCode = "RHI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4565H",
            FirstName = "Barry",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1978, 8, 5),
            Gender = "Female",
            Crn = "B003240",
            NomisNumber = "A4101QA",
            Primary = "NOMIS",
            EstCode = "LDN",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9573P",
            FirstName = "Bruce",
            LastName = "Allen",
            DateOfBirth = new DateTime(1989, 12, 10),
            Gender = "Female",
            Crn = "B004658",
            NomisNumber = "A6701WA",
            Primary = "NOMIS",
            EstCode = "HLI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7276C",
            FirstName = "Oliver",
            LastName = "Prince",
            DateOfBirth = new DateTime(1998, 8, 16),
            Gender = "Female",
            Crn = "B005676",
            NomisNumber = "A8257OA",
            Primary = "NOMIS",
            EstCode = "EYI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7041Z",
            FirstName = "Bruce",
            LastName = "Stone",
            DateOfBirth = new DateTime(1997, 10, 22),
            Gender = "Male",
            Crn = "B002248",
            NomisNumber = "A8067PA",
            Primary = "NOMIS",
            EstCode = "TSI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7680G",
            FirstName = "Hal",
            LastName = "Parker",
            DateOfBirth = new DateTime(1953, 8, 1),
            Gender = "Female",
            Crn = "B005014",
            NomisNumber = "A7056PA",
            Primary = "NOMIS",
            EstCode = "MRS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9326Y",
            FirstName = "Bruce",
            LastName = "Kent",
            DateOfBirth = new DateTime(1968, 10, 20),
            Gender = "Male",
            Crn = "B003336",
            NomisNumber = "A8225DA",
            Primary = "NOMIS",
            EstCode = "PRI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4936J",
            FirstName = "Victor",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1982, 4, 12),
            Gender = "Male",
            Crn = "B006762",
            NomisNumber = "A3361EA",
            Primary = "NOMIS",
            EstCode = "ISI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7256O",
            FirstName = "Barry",
            LastName = "Parker",
            DateOfBirth = new DateTime(1972, 3, 17),
            Gender = "Female",
            Crn = "B001484",
            NomisNumber = "A6272WA",
            Primary = "NOMIS",
            EstCode = "N03",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9960M",
            FirstName = "Bruce",
            LastName = "Lance",
            DateOfBirth = new DateTime(1969, 3, 4),
            Gender = "Female",
            Crn = "B005605",
            NomisNumber = "A3591SA",
            Primary = "NOMIS",
            EstCode = "BNI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8432I",
            FirstName = "Hal",
            LastName = "Curry",
            DateOfBirth = new DateTime(1950, 7, 15),
            Gender = "Male",
            Crn = "B007158",
            NomisNumber = "A4526NA",
            Primary = "NOMIS",
            EstCode = "SWM",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4595K",
            FirstName = "Clark",
            LastName = "Kent",
            DateOfBirth = new DateTime(1982, 9, 9),
            Gender = "Female",
            Crn = "B009462",
            NomisNumber = "A3837UA",
            Primary = "NOMIS",
            EstCode = "CFI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG4261Y",
            FirstName = "Diana",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1997, 7, 25),
            Gender = "Male",
            Crn = "B005300",
            NomisNumber = "A5265WA",
            Primary = "NOMIS",
            EstCode = "ASI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2622V",
            FirstName = "Dinah",
            LastName = "Jordan",
            DateOfBirth = new DateTime(1953, 2, 10),
            Gender = "Male",
            Crn = "B002298",
            NomisNumber = "A9419JA",
            Primary = "NOMIS",
            EstCode = "LNS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2457U",
            FirstName = "Bruce",
            LastName = "Wayne",
            DateOfBirth = new DateTime(1970, 11, 3),
            Gender = "Female",
            Crn = "B004351",
            NomisNumber = "A7217CA",
            Primary = "NOMIS",
            EstCode = "BCI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8391O",
            FirstName = "Hal",
            LastName = "Kent",
            DateOfBirth = new DateTime(2004, 5, 28),
            Gender = "Male",
            Crn = "B004107",
            NomisNumber = "A3664KA",
            Primary = "NOMIS",
            EstCode = "FBI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG2959S",
            FirstName = "Clark",
            LastName = "Lance",
            DateOfBirth = new DateTime(1951, 1, 27),
            Gender = "Male",
            Crn = "B005811",
            NomisNumber = "A8953AA",
            Primary = "NOMIS",
            EstCode = "N24",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6390M",
            FirstName = "Arthur",
            LastName = "Lance",
            DateOfBirth = new DateTime(1998, 9, 15),
            Gender = "Male",
            Crn = "B009365",
            NomisNumber = "A4773VA",
            Primary = "NOMIS",
            EstCode = "WRI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG8062R",
            FirstName = "Arthur",
            LastName = "Kent",
            DateOfBirth = new DateTime(1988, 5, 27),
            Gender = "Female",
            Crn = "B009001",
            NomisNumber = "A2146ZA",
            Primary = "NOMIS",
            EstCode = "PFI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9884C",
            FirstName = "Oliver",
            LastName = "Lance",
            DateOfBirth = new DateTime(1979, 9, 15),
            Gender = "Male",
            Crn = "B009645",
            NomisNumber = "A9574HA",
            Primary = "NOMIS",
            EstCode = "N58",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG7900F",
            FirstName = "Barry",
            LastName = "Curry",
            DateOfBirth = new DateTime(1968, 4, 13),
            Gender = "Male",
            Crn = "B009738",
            NomisNumber = "A6760EA",
            Primary = "NOMIS",
            EstCode = "N51",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG3726I",
            FirstName = "Clark",
            LastName = "Stone",
            DateOfBirth = new DateTime(1974, 9, 11),
            Gender = "Female",
            Crn = "B005227",
            NomisNumber = "A8764JA",
            Primary = "NOMIS",
            EstCode = "DHI",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG9798U",
            FirstName = "Hal",
            LastName = "Queen",
            DateOfBirth = new DateTime(1996, 5, 7),
            Gender = "Female",
            Crn = "B003704",
            NomisNumber = "A3850ZA",
            Primary = "NOMIS",
            EstCode = "T01",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        },
        new CandidateDto
        {
            Identifier = "1CFG6059P",
            FirstName = "Oliver",
            LastName = "Kent",
            DateOfBirth = new DateTime(2002, 10, 26),
            Gender = "Female",
            Crn = "B008239",
            NomisNumber = "A6307BA",
            Primary = "NOMIS",
            EstCode = "HPS",
            Nationality = "",
            Ethnicity = "",
            IsActive = true
        }
    ];

    public async Task<CandidateDto?> GetByUpciAsync(string upci)
    {
        var candidate = Candidates.SingleOrDefault(c => c.Identifier.Equals(upci, StringComparison.CurrentCultureIgnoreCase));

        if (candidate is not null)
        {
            var locationMapping = candidate.Primary switch
            {
                "NOMIS" => (Code: candidate.EstCode, Type: "Prison"),
                "DELIUS" => (Code: candidate.OrgCode, Type: "Probation"),
                _ => (null, null)
            };

            var query = from dl in unitOfWork.DbContext.LocationMappings.AsNoTracking()
                        where dl.Code == locationMapping.Code  // && dl.CodeType == locationMapping.Type
                        select new
                        {
                            dl.Code,
                            dl.CodeType,
                            dl.Description,
                            dl.DeliveryRegion,
                            dl.Location
                        };

            var location = await query.FirstOrDefaultAsync();

            candidate.LocationDescription = location switch
            {
                { Location: not null } => location.Location.Name,
                { Code: not null } => $"Unmapped Location ({location.Code} - {location.DeliveryRegion} - {location.Description})",
                _ => "Unmapped Location",
            };

            candidate.MappedLocationId = location switch
            {
                { Location: not null } => location.Location.Id,
                _ => 0
            };

        }

        return await Task.FromResult(candidate);
    }

    public async Task<IEnumerable<SearchResult>?> SearchAsync(CandidateSearchQuery searchQuery)
    {
        var lastName = searchQuery.LastName;
        var identifier = searchQuery.ExternalIdentifier;
        var dateOfBirth = DateOnly.FromDateTime((DateTime)searchQuery.DateOfBirth!);

        var blocks = Candidates
            .Where(e => e.LastName.Equals(searchQuery.LastName, StringComparison.CurrentCultureIgnoreCase) && e.DateOfBirth == searchQuery.DateOfBirth)
            .Union
            (
            Candidates.Where(e => new[]
            {
                e.Crn, e.NomisNumber
            }.Contains(searchQuery.ExternalIdentifier.ToUpper()))
            );

        if (blocks.Count() is 0)
        {
            return [];
        }

        var scores = blocks.Select(block => Score((identifier, lastName, dateOfBirth), block.Crn ?? string.Empty, block))
            .Union
            (
            blocks.Select(block => Score((identifier, lastName, dateOfBirth), block.NomisNumber ?? string.Empty, block))
            )
            .GroupBy(result => result.Upci)
            .Select(result => new SearchResult(result.Key, result.Min(r => r.Precedence)));

        return await Task.FromResult(scores);
    }

    private static SearchResult Score((string externalIdentifier, string lastName, DateOnly dateOfBirth) query, string identifier, CandidateDto block) =>
        new(block.Identifier, Precedence.GetPrecedence
        (
        (
            query.externalIdentifier,
            identifier
        ),
        (
            query.lastName,
            block.LastName
        ),
        (
            query.dateOfBirth,
            DateOnly.FromDateTime(block.DateOfBirth)
        )
        ));
}
