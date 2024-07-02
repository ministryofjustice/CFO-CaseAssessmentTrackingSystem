using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentLocation : SmartEnum<AssessmentLocation>
{
    public static readonly AssessmentLocation Unknown = new("Unknown", 0);
    public static readonly AssessmentLocation NorthWest = new("North West", 1);
    public static readonly AssessmentLocation NorthEast = new("North East", 2);
    public static readonly AssessmentLocation YorkshireAndHumberside = new("Yorkshire and Humberside", 3);
    public static readonly AssessmentLocation WestMidlands = new("West Midlands", 4);
    public static readonly AssessmentLocation EastMidlands = new("East Midlands", 5);
    public static readonly AssessmentLocation EastOfEngland = new("East of England", 6);
    public static readonly AssessmentLocation London = new("London", 7);
    public static readonly AssessmentLocation SouthWest = new("South West", 8);
    public static readonly AssessmentLocation SouthEast = new("South East", 9);

    private AssessmentLocation(string name, int value) : base(name, value)
    {
    }

    public AssessmentLocation FromContract(Contract contract)
    {
        var lotNumber = contract.LotNumber;
        var value = TryFromValue(lotNumber, out var assessmentLocation);
        return assessmentLocation ?? Unknown;
    }
}