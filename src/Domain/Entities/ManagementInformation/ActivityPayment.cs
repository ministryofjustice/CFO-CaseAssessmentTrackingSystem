using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ActivityPayment
{
#pragma warning disable CS8618 
    private ActivityPayment() { }
#pragma warning restore CS8618

    public static ActivityPayment CreateNonPayableActivityPayment(Activity activity, IneligibilityReason ineligibleReason)
    {
        var dates = new[]
        {
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            activity.CompletedOn!.Value.Date
        };

        return new ActivityPayment
        {
            Id = Guid.CreateVersion7(),
            ActivityId = activity.Id,
            ActivityCategory = activity.Category.Name,
            ActivityType = activity.Type.Name,
            CreatedOn = DateTime.UtcNow,
            ActivityApproved = activity.CompletedOn!.Value.Date,
            ParticipantId = activity.ParticipantId,
            ContractId = activity.TookPlaceAtContract.Id,
            LocationId = activity.TookPlaceAtLocation.Id,
            LocationType = activity.TookPlaceAtLocation.LocationType.Name,
            TenantId = activity.TenantId,
            EligibleForPayment = false,
            IneligibilityReason = ineligibleReason.Value,
            // in this case, an "unapproved payment" the payment period is the approval date
            PaymentPeriod = dates.Max(),
            CommencedDate = activity.CommencedOn,
            ActivityInput = activity.Created!.Value
        };
    }

    public static ActivityPayment CreatePayableActivityPayment(Activity activity, DateTime enrolmentApprovalDate)
    {
        var dates = new []
        {
            activity.CompletedOn!.Value.Date, 
            enrolmentApprovalDate.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
        };
        
        return new ActivityPayment()
        {
            Id = Guid.CreateVersion7(),
            ActivityId = activity.Id,
            ActivityCategory = activity.Category.Name,
            ActivityType = activity.Type.Name,
            CreatedOn = DateTime.UtcNow,
            ActivityApproved = activity.CompletedOn!.Value.Date,
            ParticipantId = activity.ParticipantId,
            ContractId = activity.TookPlaceAtContract.Id,
            LocationId = activity.TookPlaceAtLocation.Id,
            LocationType = activity.TookPlaceAtLocation.LocationType.Name,
            TenantId = activity.TenantId,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max(),
            CommencedDate = activity.CommencedOn,
            ActivityInput = activity.Created!.Value
        };
    }

    public required Guid Id { get; set; } 
    public required Guid ActivityId { get; set; }
    public required string ActivityCategory { get; set; }
    public required string ActivityType { get; set; } 

    /// <summary>
    /// The date this payment entry was created
    /// </summary>
    public required DateTime CreatedOn { get; set; }

    /// <summary>
    /// The date the activity commenced
    /// </summary>
    public required DateTime CommencedDate { get; set; }

    /// <summary>
    /// The date the activity was created
    /// </summary>
    public required DateTime ActivityInput { get; set; }

    /// <summary>
    /// The date the activity was approved
    /// </summary>
    public required DateTime ActivityApproved { get; set; }
    public required string ParticipantId { get; set; }
    public required string ContractId { get; set; }
    public required int LocationId { get; set; }
    public required string LocationType { get; set; }
    public required string TenantId { get; set; }
    public required bool EligibleForPayment { get; set; }
    public required string? IneligibilityReason { get; set; }
    public required DateTime PaymentPeriod { get; set; }
}