using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class PathwayPlanReviewReason : SmartEnum<PathwayPlanReviewReason>
{
    public static readonly PathwayPlanReviewReason Default = new("Please Select Reason", -1);
    public static readonly PathwayPlanReviewReason OriginalReview = new("Original review", 0);
    public static readonly PathwayPlanReviewReason AssessmentPerformed = new("Assessment Changed", 1);
    public static readonly PathwayPlanReviewReason ParticipantChangedLocation = new("Location Change", 2);
    public static readonly PathwayPlanReviewReason ChangeToCircumstances = new("Change Of Personal Circumstances", 3);
    public static readonly PathwayPlanReviewReason EndOfWingPhaseDelivery = new("End Of Wing Phase Delivery",4);
    public static readonly PathwayPlanReviewReason NinetyDayReview = new("90 Day Review", 5);
    public static readonly PathwayPlanReviewReason InitialReview = new ("Initial Review", 6);
    public static readonly PathwayPlanReviewReason Reassignment = new("Reassignment", 7);
    
    private PathwayPlanReviewReason(string name, int value) 
        : base(name, value) 
    {
    }
    
    public bool IsValidSelection() => this != Default;

    public static PathwayPlanReviewReason[] OrderedList => new[]
    {
        Default,
        NinetyDayReview,
        AssessmentPerformed,
        ChangeToCircumstances,
        EndOfWingPhaseDelivery,
        InitialReview,
        ParticipantChangedLocation,
        Reassignment
    };
}