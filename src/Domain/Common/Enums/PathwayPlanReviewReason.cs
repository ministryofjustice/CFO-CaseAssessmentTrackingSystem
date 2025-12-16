using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class PathwayPlanReviewReason : SmartEnum<PathwayPlanReviewReason>
{
    public static readonly PathwayPlanReviewReason Default =  new("Please Select Reason", -1);
    public static readonly PathwayPlanReviewReason AssessmentPerformed = new("Assessment performed", 0);
    public static readonly PathwayPlanReviewReason ParticipantChangedLocation = new("Participant changed location", 1);
    public static readonly PathwayPlanReviewReason ChangeToCircumstances = new("Change of participant personal circumstances", 2);
    public static readonly PathwayPlanReviewReason EndOfWingPhaseDelivery = new("End of wing phase delivery",3);
    public static readonly PathwayPlanReviewReason NinetyDayReview = new("90 day review", 4);

    private PathwayPlanReviewReason(string name, int value) 
        : base(name, value) 
    {
    }
    
    public bool IsValidSelection()
    {
        return this != Default;
    }

    public void ThrowIfInvalid()
    {
        if (!IsValidSelection())
        {
            throw new InvalidOperationException("A review reason must be selected.");
        }
    }
}