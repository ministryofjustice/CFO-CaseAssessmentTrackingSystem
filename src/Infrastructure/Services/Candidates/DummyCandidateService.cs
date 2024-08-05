using Cfo.Cats.Application.Features.Candidates.DTOs;
using Cfo.Cats.Application.Features.Candidates.Queries.Search;
using Matching.Core.Search;


namespace Cfo.Cats.Infrastructure.Services.Candidates;

public class DummyCandidateService : ICandidateService
{
    IReadOnlyList<CandidateDto> Candidates =>
    [
        
    new CandidateDto
    {
        Identifier = "1CFG1109X",
        FirstName = "Barry",
        LastName = "Stone",
        DateOfBirth = new DateTime(1979, 01, 18),
        Gender = "Male",
        Crn = "B008622",
        NomisNumber = "A7022B",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Eastwood Park",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3305M",
        FirstName = "Clark",
        LastName = "Queen",
        DateOfBirth = new DateTime(1985, 07, 16),
        Gender = "Female",
        Crn = "B004786",
        NomisNumber = "A7373X",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Southampton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3647Z",
        FirstName = "Hal",
        LastName = "Parker",
        DateOfBirth = new DateTime(2000, 08, 31),
        Gender = "Male",
        Crn = "B006754",
        NomisNumber = "A1118O",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Guys Marsh",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4819M",
        FirstName = "Arthur",
        LastName = "Stone",
        DateOfBirth = new DateTime(1961, 03, 08),
        Gender = "Female",
        Crn = "B008866",
        NomisNumber = "A6972X",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stoke",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3977A",
        FirstName = "Oliver",
        LastName = "Kent",
        DateOfBirth = new DateTime(1966, 08, 16),
        Gender = "Male",
        Crn = "B004529",
        NomisNumber = "A3749H",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lancaster",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4580T",
        FirstName = "Barry",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1979, 06, 18),
        Gender = "Male",
        Crn = "B007498",
        NomisNumber = "A6102N",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Northumberland",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7754K",
        FirstName = "Victor",
        LastName = "Kent",
        DateOfBirth = new DateTime(1956, 10, 21),
        Gender = "Male",
        Crn = "B003913",
        NomisNumber = "A2817B",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Brixton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1873Q",
        FirstName = "Arthur",
        LastName = "Wayne",
        DateOfBirth = new DateTime(1988, 08, 15),
        Gender = "Male",
        Crn = "B004044",
        NomisNumber = "A8654X",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Peterborough (M)",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5221C",
        FirstName = "Victor",
        LastName = "Parker",
        DateOfBirth = new DateTime(1977, 09, 14),
        Gender = "Male",
        Crn = "B003506",
        NomisNumber = "A3576S",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lewisham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5006P",
        FirstName = "Clark",
        LastName = "Kent",
        DateOfBirth = new DateTime(1988, 09, 09),
        Gender = "Female",
        Crn = "B001407",
        NomisNumber = "A8065E",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Exeter",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2001T",
        FirstName = "Oliver",
        LastName = "Jordan",
        DateOfBirth = new DateTime(2004, 09, 30),
        Gender = "Female",
        Crn = "B003237",
        NomisNumber = "A6587Y",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Portland",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5437L",
        FirstName = "Hal",
        LastName = "Parker",
        DateOfBirth = new DateTime(1956, 09, 25),
        Gender = "Male",
        Crn = "B003171",
        NomisNumber = "A6952Z",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "High Down",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2883L",
        FirstName = "Oliver",
        LastName = "Prince",
        DateOfBirth = new DateTime(1961, 08, 29),
        Gender = "Male",
        Crn = "B007887",
        NomisNumber = "A3402I",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Guys Marsh",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2167R",
        FirstName = "Bruce",
        LastName = "Allen",
        DateOfBirth = new DateTime(1969, 08, 27),
        Gender = "Female",
        Crn = "B009234",
        NomisNumber = "A7364G",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stoke",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9064R",
        FirstName = "Bruce",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1982, 07, 19),
        Gender = "Male",
        Crn = "B004752",
        NomisNumber = "A6222S",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Leeds",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1900I",
        FirstName = "Oliver",
        LastName = "Curry",
        DateOfBirth = new DateTime(1983, 06, 30),
        Gender = "Male",
        Crn = "B005270",
        NomisNumber = "A9293C",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Leeds",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2847D",
        FirstName = "Bruce",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1991, 09, 17),
        Gender = "Female",
        Crn = "B005459",
        NomisNumber = "A5884J",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Peterborough",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7892B",
        FirstName = "Arthur",
        LastName = "Wayne",
        DateOfBirth = new DateTime(1952, 08, 22),
        Gender = "Male",
        Crn = "B006933",
        NomisNumber = "A9162Q",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Leeds",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6906H",
        FirstName = "Diana",
        LastName = "Curry",
        DateOfBirth = new DateTime(1980, 08, 09),
        Gender = "Male",
        Crn = "B005260",
        NomisNumber = "A6150I",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Hull",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1333S",
        FirstName = "Peter",
        LastName = "Allen",
        DateOfBirth = new DateTime(1989, 12, 02),
        Gender = "Female",
        Crn = "B002552",
        NomisNumber = "A2257F",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Haverigg",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1479H",
        FirstName = "Peter",
        LastName = "Prince",
        DateOfBirth = new DateTime(1956, 07, 18),
        Gender = "Female",
        Crn = "B004442",
        NomisNumber = "A6948J",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Woodhill",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7788V",
        FirstName = "Diana",
        LastName = "Lance",
        DateOfBirth = new DateTime(1953, 10, 27),
        Gender = "Male",
        Crn = "B004219",
        NomisNumber = "A9649A",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Liverpool",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1553F",
        FirstName = "Victor",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1978, 12, 08),
        Gender = "Female",
        Crn = "B008576",
        NomisNumber = "A9860R",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Hindley",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4867Q",
        FirstName = "Oliver",
        LastName = "Jordan",
        DateOfBirth = new DateTime(2000, 02, 03),
        Gender = "Male",
        Crn = "B008340",
        NomisNumber = "A1567L",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lancaster",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5854G",
        FirstName = "Dinah",
        LastName = "Queen",
        DateOfBirth = new DateTime(1952, 10, 09),
        Gender = "Female",
        Crn = "B009885",
        NomisNumber = "A9937F",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Ranby",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5469L",
        FirstName = "Peter",
        LastName = "Queen",
        DateOfBirth = new DateTime(1953, 11, 18),
        Gender = "Male",
        Crn = "B001257",
        NomisNumber = "A4692D",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Peterborough (F)",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5502H",
        FirstName = "Bruce",
        LastName = "Kent",
        DateOfBirth = new DateTime(1978, 02, 11),
        Gender = "Male",
        Crn = "B009194",
        NomisNumber = "A1411Y",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "North West Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2825E",
        FirstName = "Peter",
        LastName = "Prince",
        DateOfBirth = new DateTime(1975, 12, 23),
        Gender = "Female",
        Crn = "B009268",
        NomisNumber = "A7907D",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Thameside",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8516T",
        FirstName = "Dinah",
        LastName = "Kent",
        DateOfBirth = new DateTime(1954, 06, 19),
        Gender = "Female",
        Crn = "B009761",
        NomisNumber = "A7400N",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Brixton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5298O",
        FirstName = "Victor",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1961, 07, 28),
        Gender = "Male",
        Crn = "B007256",
        NomisNumber = "A8430T",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Bullingdon",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9775P",
        FirstName = "Barry",
        LastName = "Stone",
        DateOfBirth = new DateTime(1960, 08, 14),
        Gender = "Female",
        Crn = "B008155",
        NomisNumber = "A1067S",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Wealstun",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6849L",
        FirstName = "Bruce",
        LastName = "Parker",
        DateOfBirth = new DateTime(1977, 02, 20),
        Gender = "Male",
        Crn = "B009597",
        NomisNumber = "A7483A",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "East Sutton Park",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8347B",
        FirstName = "Clark",
        LastName = "Queen",
        DateOfBirth = new DateTime(2005, 10, 06),
        Gender = "Female",
        Crn = "B005305",
        NomisNumber = "A1056J",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "South West Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4189E",
        FirstName = "Bruce",
        LastName = "Curry",
        DateOfBirth = new DateTime(1956, 04, 14),
        Gender = "Female",
        Crn = "B006480",
        NomisNumber = "A3064C",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stoke Heath",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5514M",
        FirstName = "Clark",
        LastName = "Curry",
        DateOfBirth = new DateTime(1962, 05, 28),
        Gender = "Female",
        Crn = "B005528",
        NomisNumber = "A7236E",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Channings Wood",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5252T",
        FirstName = "Peter",
        LastName = "Queen",
        DateOfBirth = new DateTime(1964, 09, 23),
        Gender = "Female",
        Crn = "B008719",
        NomisNumber = "A4570W",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Newcastle",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6789D",
        FirstName = "Clark",
        LastName = "Parker",
        DateOfBirth = new DateTime(1968, 10, 24),
        Gender = "Male",
        Crn = "B002584",
        NomisNumber = "A6440G",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Send",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1848U",
        FirstName = "Barry",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1981, 06, 09),
        Gender = "Female",
        Crn = "B004852",
        NomisNumber = "A7126Q",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "South West Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4892S",
        FirstName = "Arthur",
        LastName = "Allen",
        DateOfBirth = new DateTime(1960, 10, 09),
        Gender = "Male",
        Crn = "B008270",
        NomisNumber = "A6249N",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Doncaster",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5866O",
        FirstName = "Peter",
        LastName = "Jordan",
        DateOfBirth = new DateTime(2001, 10, 07),
        Gender = "Female",
        Crn = "B008561",
        NomisNumber = "A9186B",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Hindley",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7842D",
        FirstName = "Diana",
        LastName = "Queen",
        DateOfBirth = new DateTime(1982, 07, 15),
        Gender = "Female",
        Crn = "B005033",
        NomisNumber = "A2699Z",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "East Midlands Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8262O",
        FirstName = "Peter",
        LastName = "Curry",
        DateOfBirth = new DateTime(1984, 07, 02),
        Gender = "Male",
        Crn = "B001022",
        NomisNumber = "A2865U",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Dartmoor",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6353O",
        FirstName = "Hal",
        LastName = "Stone",
        DateOfBirth = new DateTime(1952, 01, 15),
        Gender = "Male",
        Crn = "B002433",
        NomisNumber = "A4973L",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "South West Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2308N",
        FirstName = "Clark",
        LastName = "Parker",
        DateOfBirth = new DateTime(2004, 06, 26),
        Gender = "Male",
        Crn = "B006501",
        NomisNumber = "A3961U",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Pentonville",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8867D",
        FirstName = "Arthur",
        LastName = "Curry",
        DateOfBirth = new DateTime(1951, 09, 20),
        Gender = "Male",
        Crn = "B009689",
        NomisNumber = "A8052R",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Doncaster",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5131C",
        FirstName = "Barry",
        LastName = "Allen",
        DateOfBirth = new DateTime(1981, 06, 26),
        Gender = "Male",
        Crn = "B005370",
        NomisNumber = "A5295T",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Wayland",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8864M",
        FirstName = "Arthur",
        LastName = "Curry",
        DateOfBirth = new DateTime(1985, 03, 14),
        Gender = "Female",
        Crn = "B001879",
        NomisNumber = "A4162O",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Hull",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5544Z",
        FirstName = "Hal",
        LastName = "Allen",
        DateOfBirth = new DateTime(1954, 08, 11),
        Gender = "Female",
        Crn = "B007141",
        NomisNumber = "A3667G",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Luton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9475S",
        FirstName = "Victor",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1950, 04, 10),
        Gender = "Female",
        Crn = "B009820",
        NomisNumber = "A1011F",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Elmley",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8570M",
        FirstName = "Bruce",
        LastName = "Queen",
        DateOfBirth = new DateTime(1951, 07, 21),
        Gender = "Male",
        Crn = "B005022",
        NomisNumber = "A2364M",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Woodhill",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1646R",
        FirstName = "Hal",
        LastName = "Kent",
        DateOfBirth = new DateTime(1957, 08, 21),
        Gender = "Female",
        Crn = "B008390",
        NomisNumber = "A9321H",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Brinsford",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7199D",
        FirstName = "Hal",
        LastName = "Prince",
        DateOfBirth = new DateTime(1990, 06, 25),
        Gender = "Female",
        Crn = "B006153",
        NomisNumber = "A2303F",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Durham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8089M",
        FirstName = "Barry",
        LastName = "Parker",
        DateOfBirth = new DateTime(1982, 10, 31),
        Gender = "Female",
        Crn = "B005799",
        NomisNumber = "A8614G",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Kirklevington Grange",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5547B",
        FirstName = "Barry",
        LastName = "Lance",
        DateOfBirth = new DateTime(1990, 01, 17),
        Gender = "Female",
        Crn = "B002156",
        NomisNumber = "A7019I",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lambeth",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2192B",
        FirstName = "Victor",
        LastName = "Allen",
        DateOfBirth = new DateTime(1999, 07, 30),
        Gender = "Female",
        Crn = "B009460",
        NomisNumber = "A2301T",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Croydon",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9918N",
        FirstName = "Barry",
        LastName = "Stone",
        DateOfBirth = new DateTime(1962, 03, 25),
        Gender = "Male",
        Crn = "B007272",
        NomisNumber = "A7368N",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lewisham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1461R",
        FirstName = "Peter",
        LastName = "Parker",
        DateOfBirth = new DateTime(1995, 03, 25),
        Gender = "Male",
        Crn = "B004903",
        NomisNumber = "A9368J",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Humber",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2867U",
        FirstName = "Diana",
        LastName = "Lance",
        DateOfBirth = new DateTime(1968, 06, 27),
        Gender = "Male",
        Crn = "B009542",
        NomisNumber = "A4368J",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Askham Grange",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3069P",
        FirstName = "Dinah",
        LastName = "Stone",
        DateOfBirth = new DateTime(1987, 06, 22),
        Gender = "Female",
        Crn = "B009809",
        NomisNumber = "A1436V",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lindholme",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7072K",
        FirstName = "Arthur",
        LastName = "Queen",
        DateOfBirth = new DateTime(1975, 10, 05),
        Gender = "Male",
        Crn = "B009199",
        NomisNumber = "A6247X",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stanford Hill",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2438I",
        FirstName = "Barry",
        LastName = "Prince",
        DateOfBirth = new DateTime(1971, 07, 08),
        Gender = "Female",
        Crn = "B003456",
        NomisNumber = "A5176N",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Highpoint",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6580Z",
        FirstName = "Dinah",
        LastName = "Allen",
        DateOfBirth = new DateTime(1960, 03, 05),
        Gender = "Female",
        Crn = "B003215",
        NomisNumber = "A4206S",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Yorkshire and Humberside Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1303N",
        FirstName = "Diana",
        LastName = "Lance",
        DateOfBirth = new DateTime(1984, 05, 05),
        Gender = "Male",
        Crn = "B002668",
        NomisNumber = "A5319A",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Humber",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2282K",
        FirstName = "Barry",
        LastName = "Lance",
        DateOfBirth = new DateTime(1960, 11, 30),
        Gender = "Female",
        Crn = "B006078",
        NomisNumber = "A8725W",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Wolverhampton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7502D",
        FirstName = "Hal",
        LastName = "Prince",
        DateOfBirth = new DateTime(1965, 09, 03),
        Gender = "Male",
        Crn = "B005846",
        NomisNumber = "A7652N",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Haverigg",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8236D",
        FirstName = "Bruce",
        LastName = "Allen",
        DateOfBirth = new DateTime(1999, 02, 24),
        Gender = "Male",
        Crn = "B001005",
        NomisNumber = "A6250U",
        Nationality = "American",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Haverigg",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4926U",
        FirstName = "Peter",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1990, 04, 04),
        Gender = "Female",
        Crn = "B008641",
        NomisNumber = "A3877T",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Woodhill",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3204I",
        FirstName = "Bruce",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1958, 05, 02),
        Gender = "Male",
        Crn = "B007677",
        NomisNumber = "A1403D",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "The Verne",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3962X",
        FirstName = "Arthur",
        LastName = "Parker",
        DateOfBirth = new DateTime(1981, 02, 12),
        Gender = "Female",
        Crn = "B007012",
        NomisNumber = "A6308N",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Dovegate",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7281E",
        FirstName = "Peter",
        LastName = "Stone",
        DateOfBirth = new DateTime(1958, 07, 02),
        Gender = "Male",
        Crn = "B001642",
        NomisNumber = "A6119B",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Sheffield",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1626P",
        FirstName = "Bruce",
        LastName = "Curry",
        DateOfBirth = new DateTime(1996, 05, 24),
        Gender = "Male",
        Crn = "B002835",
        NomisNumber = "A5610C",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Liverpool",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1773W",
        FirstName = "Peter",
        LastName = "Allen",
        DateOfBirth = new DateTime(2005, 07, 21),
        Gender = "Male",
        Crn = "B007895",
        NomisNumber = "A9218R",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Humber",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG1359X",
        FirstName = "Bruce",
        LastName = "Parker",
        DateOfBirth = new DateTime(1954, 02, 23),
        Gender = "Male",
        Crn = "B004099",
        NomisNumber = "A5852K",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Channings Wood",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5462D",
        FirstName = "Oliver",
        LastName = "Queen",
        DateOfBirth = new DateTime(1975, 07, 10),
        Gender = "Female",
        Crn = "B007065",
        NomisNumber = "A6399O",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Birmingham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG5813Z",
        FirstName = "Hal",
        LastName = "Wayne",
        DateOfBirth = new DateTime(1969, 12, 28),
        Gender = "Male",
        Crn = "B004062",
        NomisNumber = "A7138M",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Nottingham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2975B",
        FirstName = "Peter",
        LastName = "Queen",
        DateOfBirth = new DateTime(1996, 02, 13),
        Gender = "Male",
        Crn = "B009564",
        NomisNumber = "A3824O",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Yorkshire and Humberside Community",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7191X",
        FirstName = "Dinah",
        LastName = "Parker",
        DateOfBirth = new DateTime(1992, 06, 27),
        Gender = "Male",
        Crn = "B002061",
        NomisNumber = "A4383X",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stocken",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4565H",
        FirstName = "Barry",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1978, 08, 05),
        Gender = "Female",
        Crn = "B003240",
        NomisNumber = "A4101Q",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Askham Grange",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9573P",
        FirstName = "Bruce",
        LastName = "Allen",
        DateOfBirth = new DateTime(1989, 12, 10),
        Gender = "Female",
        Crn = "B004658",
        NomisNumber = "A6701W",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Peterborough (M)",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7276C",
        FirstName = "Oliver",
        LastName = "Prince",
        DateOfBirth = new DateTime(1998, 08, 16),
        Gender = "Female",
        Crn = "B005676",
        NomisNumber = "A8257O",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Lewisham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7041Z",
        FirstName = "Bruce",
        LastName = "Stone",
        DateOfBirth = new DateTime(1997, 10, 22),
        Gender = "Male",
        Crn = "B002248",
        NomisNumber = "A8067P",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Thorn Cross",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7680G",
        FirstName = "Hal",
        LastName = "Parker",
        DateOfBirth = new DateTime(1953, 08, 01),
        Gender = "Female",
        Crn = "B005014",
        NomisNumber = "A7056P",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Newcastle",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9326Y",
        FirstName = "Bruce",
        LastName = "Kent",
        DateOfBirth = new DateTime(1968, 10, 20),
        Gender = "Male",
        Crn = "B003336",
        NomisNumber = "A8225D",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Forest Bank",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4936J",
        FirstName = "Victor",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1982, 04, 12),
        Gender = "Male",
        Crn = "B006762",
        NomisNumber = "A3361E",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Low Newton",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7256O",
        FirstName = "Barry",
        LastName = "Parker",
        DateOfBirth = new DateTime(1972, 03, 17),
        Gender = "Female",
        Crn = "B001484",
        NomisNumber = "A6272W",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Bullingdon",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9960M",
        FirstName = "Bruce",
        LastName = "Lance",
        DateOfBirth = new DateTime(1969, 03, 04),
        Gender = "Female",
        Crn = "B005605",
        NomisNumber = "A3591S",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Humber",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8432I",
        FirstName = "Hal",
        LastName = "Curry",
        DateOfBirth = new DateTime(1950, 07, 15),
        Gender = "Male",
        Crn = "B007158",
        NomisNumber = "A4526N",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Stocken",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4595K",
        FirstName = "Clark",
        LastName = "Kent",
        DateOfBirth = new DateTime(1982, 09, 09),
        Gender = "Female",
        Crn = "B009462",
        NomisNumber = "A3837U",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Dovegate",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG4261Y",
        FirstName = "Diana",
        LastName = "Wayne",
        DateOfBirth = new DateTime(1997, 07, 25),
        Gender = "Male",
        Crn = "B005300",
        NomisNumber = "A5265W",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Durham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2622V",
        FirstName = "Dinah",
        LastName = "Jordan",
        DateOfBirth = new DateTime(1953, 02, 10),
        Gender = "Male",
        Crn = "B002298",
        NomisNumber = "A9419J",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Sunderland",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2457U",
        FirstName = "Bruce",
        LastName = "Wayne",
        DateOfBirth = new DateTime(1970, 11, 03),
        Gender = "Female",
        Crn = "B004351",
        NomisNumber = "A7217C",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Isis",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8391O",
        FirstName = "Hal",
        LastName = "Kent",
        DateOfBirth = new DateTime(2004, 05, 28),
        Gender = "Male",
        Crn = "B004107",
        NomisNumber = "A3664K",
        Nationality = "German",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "East Sutton Park",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG2959S",
        FirstName = "Clark",
        LastName = "Lance",
        DateOfBirth = new DateTime(1951, 01, 27),
        Gender = "Male",
        Crn = "B005811",
        NomisNumber = "A8953A",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Rochester",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6390M",
        FirstName = "Arthur",
        LastName = "Lance",
        DateOfBirth = new DateTime(1998, 09, 15),
        Gender = "Male",
        Crn = "B009365",
        NomisNumber = "A4773V",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Featherstone",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG8062R",
        FirstName = "Arthur",
        LastName = "Kent",
        DateOfBirth = new DateTime(1988, 05, 27),
        Gender = "Female",
        Crn = "B009001",
        NomisNumber = "A2146Z",
        Nationality = "Canadian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Winchester",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9884C",
        FirstName = "Oliver",
        LastName = "Lance",
        DateOfBirth = new DateTime(1979, 09, 15),
        Gender = "Male",
        Crn = "B009645",
        NomisNumber = "A9574H",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Liverpool",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG7900F",
        FirstName = "Barry",
        LastName = "Curry",
        DateOfBirth = new DateTime(1968, 04, 13),
        Gender = "Male",
        Crn = "B009738",
        NomisNumber = "A6760E",
        Nationality = "French",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Huddersfield",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG3726I",
        FirstName = "Clark",
        LastName = "Stone",
        DateOfBirth = new DateTime(1974, 09, 11),
        Gender = "Female",
        Crn = "B005227",
        NomisNumber = "A8764J",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Wealstun",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG9798U",
        FirstName = "Hal",
        LastName = "Queen",
        DateOfBirth = new DateTime(1996, 05, 07),
        Gender = "Female",
        Crn = "B003704",
        NomisNumber = "A3850Z",
        Nationality = "Australian",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "Nottingham",
    },
    
    new CandidateDto
    {
        Identifier = "1CFG6059P",
        FirstName = "Oliver",
        LastName = "Kent",
        DateOfBirth = new DateTime(2002, 10, 26),
        Gender = "Female",
        Crn = "B008239",
        NomisNumber = "A6307B",
        Nationality = "British",
        Ethnicity = "",
        Primary = "Nomis",
        CurrentLocation = "London Community",
    },
    
    ];

    public async Task<CandidateDto?> GetByUpciAsync(string upci)
    {
        var candidate = Candidates.SingleOrDefault(c => c.Identifier.Equals(upci, StringComparison.CurrentCultureIgnoreCase));
        return await Task.FromResult(candidate);
    }

    public async Task<IEnumerable<SearchResult>?> SearchAsync(CandidateSearchQuery searchQuery)
    {
        string lastName = searchQuery.LastName;
        string identifier = searchQuery.ExternalIdentifier;
        DateOnly dateOfBirth = DateOnly.FromDateTime((DateTime)searchQuery.DateOfBirth!);

        var blocks = Candidates
            .Where(e => e.LastName.Equals(searchQuery.LastName, StringComparison.CurrentCultureIgnoreCase) && e.DateOfBirth == searchQuery.DateOfBirth)
            .Union
            (
                Candidates.Where(e => new[] { e.Crn, e.NomisNumber }.Contains(searchQuery.ExternalIdentifier.ToUpper()))
            );

        if(blocks.Count() is 0)
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

    static SearchResult Score((string externalIdentifier, string lastName, DateOnly dateOfBirth) query, string identifier, CandidateDto block) => 
        new(block.Identifier, Precedence.GetPrecedence
        (
            identifiers:
            (
                query.externalIdentifier,
                identifier
            ),
            lastNames:
            (
                query.lastName,
                block.LastName
            ),
            dateOfBirths:
            (
                query.dateOfBirth,
                DateOnly.FromDateTime(block.DateOfBirth)
            )
        ));

}
