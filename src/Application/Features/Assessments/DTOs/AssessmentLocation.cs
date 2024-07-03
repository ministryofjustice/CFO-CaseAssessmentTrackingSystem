using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;
using DocumentFormat.OpenXml.Drawing;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentLocation : SmartEnum<AssessmentLocation>
{
    public const string Unknown = "Unknown";
    public const string EastMidlands = "East Midlands";
    public const string NorthWest = "North West";
    public const string NorthEast = "North East";
    public const string YorkshireAndHumberside = "Yorkshire and Humberside";
    public const string WestMidlands = "West Midlands";
    public const string EastOfEngland = "East of England";
    public const string London = "London";
    public const string SouthWest = "South West";
    public const string SouthEast = "South East";
    
    public static readonly AssessmentLocation UnknownAssessmentLocation = new(Unknown, 0);
    public static readonly AssessmentLocation NorthWestAssessmentLocation = new(NorthWest, 1);
    public static readonly AssessmentLocation NorthEastAssessmentLocation = new(NorthEast, 2);
    public static readonly AssessmentLocation YorkshireAndHumbersideAssessmentLocation = new(YorkshireAndHumberside, 3);
    public static readonly AssessmentLocation WestMidlandsAssessmentLocation = new(WestMidlands, 4);
    public static readonly AssessmentLocation EastMidlandsAssessmentLocation = new(EastMidlands, 5);
    public static readonly AssessmentLocation EastOfEnglandAssessmentLocation = new(EastOfEngland, 6);
    public static readonly AssessmentLocation LondonAssessmentLocation = new(London, 7);
    public static readonly AssessmentLocation SouthWestAssessmentLocation = new(SouthWest, 8);
    public static readonly AssessmentLocation SouthEastAssessmentLocation = new(SouthEast, 9);

    private AssessmentLocation(string name, int value) : base(name, value)
    {
    }

    public AssessmentLocation FromContract(Contract contract)
    {
        var lotNumber = contract.LotNumber;
        var value = TryFromValue(lotNumber, out var assessmentLocation);
        return assessmentLocation ?? UnknownAssessmentLocation;
    }
}