using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public  class AssessmentLocation : SmartEnum<AssessmentLocation>
{
    public static readonly AssessmentLocation Unknown = new AssessmentLocation("Unknown", 0);
    public static readonly AssessmentLocation NorthWest = new AssessmentLocation("North West", 1);
    public static readonly AssessmentLocation NorthEast = new AssessmentLocation("North East", 2);
    public static readonly AssessmentLocation YorkshireAndHumberside = new AssessmentLocation("Yorkshire and Humberside", 3);
    public static readonly AssessmentLocation WestMidlands = new AssessmentLocation("West Midlands", 4);
    public static readonly AssessmentLocation EastMidlands = new AssessmentLocation("East Midlands", 5);
    public static readonly AssessmentLocation EastOfEngland = new AssessmentLocation("East of England", 6);
    public static readonly AssessmentLocation London = new AssessmentLocation("London", 7);
    public static readonly AssessmentLocation SouthWest = new AssessmentLocation("South West", 8);
    public static readonly AssessmentLocation SouthEast = new AssessmentLocation("South East", 9);
    
    private AssessmentLocation(string name, int value) : base(name, value)
    {
    }

    public AssessmentLocation FromContract(Contract contract)
    {
        var lotNumber = contract.LotNumber;
        var value = SmartEnum<AssessmentLocation>.TryFromValue(lotNumber, out var assessmentLocation);
        return assessmentLocation ?? AssessmentLocation.Unknown;
    }
}
