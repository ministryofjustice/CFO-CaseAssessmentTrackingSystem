using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class PathwayPlanReviewReason : SmartEnum<PathwayPlanReviewReason>
{
    public static readonly PathwayPlanReviewReason Default = new("Please Select Reason", -1);
    public static readonly PathwayPlanReviewReason OriginalReview = new("Original review", 0);
    public static readonly PathwayPlanReviewReason AssessmentPerformed = new("Assessment performed", 1);
    public static readonly PathwayPlanReviewReason ParticipantChangedLocation = new("Participant changed location", 2);
    public static readonly PathwayPlanReviewReason ChangeToCircumstances = new("Change of participant personal circumstances", 3);
    public static readonly PathwayPlanReviewReason EndOfWingPhaseDelivery = new("End of wing phase delivery",4);
    public static readonly PathwayPlanReviewReason NinetyDayReview = new("90 day review", 5);
    
    private PathwayPlanReviewReason(string name, int value) 
        : base(name, value) 
    {
    }
    
    public bool IsValidSelection() => this != Default;
}