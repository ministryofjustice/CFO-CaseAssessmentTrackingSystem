using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Exceptions;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class EmploymentPayment
{
#pragma warning disable CS8618
    private EmploymentPayment()
    {
    }
#pragma warning restore CS8618


    public static EmploymentPayment CreateNonPayableEmploymentPayment(EmploymentActivity activity, IneligibilityReason ineligibilityReason)
    {
        if (activity.ApprovedOn is null)
        {
            throw new ArgumentException("Cannot record MI for an unapproved item");
        }

        return new EmploymentPayment
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            CommencedDate = activity.CommencedOn.Date,
            ActivityInput = activity.Created!.Value,
            ActivityId = activity.Id,
            ActivityApproved = activity.ApprovedOn.Value.Date,
            ParticipantId = activity.ParticipantId,
            ContractId = activity.TookPlaceAtContract.Id,
            LocationId = activity.TookPlaceAtLocation.Id,
            LocationType = activity.TookPlaceAtLocation.LocationType.Name,
            TenantId = activity.TenantId,
            EligibleForPayment = false,
            IneligibilityReason = ineligibilityReason.Value,
            PaymentPeriod = activity.ApprovedOn.Value.Date
        };
    }

    public static EmploymentPayment CreateEmploymentPayment(EmploymentActivity activity,
        DateTime enrolmentApprovalDate)
    {
        if (activity.ApprovedOn is null)
        {
            throw new ArgumentException("Cannot record MI for an unapproved item");
        }

        var dates = new[]
        {
            activity.ApprovedOn!.Value.Date,
            enrolmentApprovalDate.Date
        };

        return new EmploymentPayment
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            CommencedDate = activity.CommencedOn.Date,
            ActivityInput = activity.Created!.Value,
            ActivityId = activity.Id,
            ActivityApproved = activity.ApprovedOn.Value.Date,
            ParticipantId = activity.ParticipantId,
            ContractId = activity.TookPlaceAtContract.Id,
            LocationId = activity.TookPlaceAtLocation.Id,
            LocationType = activity.TookPlaceAtLocation.LocationType.Name,
            TenantId = activity.TenantId,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max()
        };

    }


    public required Guid Id { get; set; } 
    public required DateTime CreatedOn { get; set; }


    /// <summary>
    /// The date the activity commenced
    /// </summary>
    public required DateTime CommencedDate { get; set; }

    /// <summary>
    /// The date the activity was created
    /// </summary>
    public required DateTime ActivityInput { get; set; }

    public required Guid ActivityId { get; set; }
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
