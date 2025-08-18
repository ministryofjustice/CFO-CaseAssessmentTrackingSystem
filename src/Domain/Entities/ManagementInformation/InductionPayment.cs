using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Inductions;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class InductionPayment
{
#pragma warning disable CS8618 
    private InductionPayment()
    {
        // required for EF Core
    }
#pragma warning restore CS8618    

    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required DateTime CreatedOn { get; set; }

    public required string ParticipantId { get; set; }

    public required string SupportWorker { get; set; }

    public required string ContractId { get; set; }

    public required DateTime Induction { get; set; }

    public required DateTime Approved { get; set; }

    public required int LocationId { get; set; }

    public string LocationType { get; set; }

    public required string TenantId { get; set; }

    public required bool EligibleForPayment { get; set; }

    public required string? IneligibilityReason { get; set; }

    public required DateTime CommencedDate { get; set; }

    public required DateTime InductionInput { get; set; }

    public required DateTime PaymentPeriod { get; set; }

    public static InductionPayment CreatePayableInductionPayment(HubInduction induction, DateTime enrolmentApprovalDate)
    {
        var dates = new[]
        {
            induction.InductionDate,
            enrolmentApprovalDate.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };
        return new InductionPayment()
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = induction.ParticipantId,
            SupportWorker = induction.CreatedBy!,
            ContractId = induction.Location!.Contract!.Id,
            Approved = dates.Max(),
            Induction = induction.InductionDate,
            LocationId = induction.LocationId,
            LocationType = "Hub",
            TenantId = induction.Owner!.Tenant!.Id,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max(),
            CommencedDate = induction.InductionDate,
            InductionInput = induction.Created!.Value,
            CreatedOn = DateTime.UtcNow
        };
    }
    public static InductionPayment CreatePayableInductionPayment(WingInduction induction, DateTime enrolmentApprovalDate)
    {
        var dates = new[]
        {
            induction.InductionDate,
            enrolmentApprovalDate.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };
        return new InductionPayment()
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = induction.ParticipantId,
            SupportWorker = induction.CreatedBy!,
            ContractId = induction.Location!.Contract!.Id,
            Approved = dates.Max(),
            Induction = induction.InductionDate,
            LocationId = induction.LocationId,
            LocationType = induction.Location.LocationType.Name,
            TenantId = induction.Location.Contract.Tenant!.Id,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max(),
            CommencedDate = induction.InductionDate,
            InductionInput = induction.Created!.Value,
            CreatedOn = DateTime.UtcNow
        };
    }

    public static InductionPayment CreateNonPayableInductionPayment(HubInduction induction, IneligibilityReason ineligibilityReason)
    {

        var dates = new[]
        {
            induction.InductionDate,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };

        return new InductionPayment()
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = induction.ParticipantId,
            SupportWorker = induction.CreatedBy!,
            ContractId = induction.Location!.Contract!.Id,
            Approved = induction.Created!.Value,
            Induction = induction.InductionDate,
            LocationId = induction.LocationId,
            LocationType = "Hub",
            TenantId = induction.Owner!.Tenant!.Id,
            EligibleForPayment = false,
            IneligibilityReason = ineligibilityReason.Value,
            PaymentPeriod = dates.Max(),
            CommencedDate = induction.InductionDate,
            InductionInput = induction.Created!.Value,
            CreatedOn = DateTime.UtcNow
        };
    }
    public static InductionPayment CreateNonPayableInductionPayment(WingInduction induction, IneligibilityReason ineligibilityReason)
    {
        var dates = new[]
        {
            induction.InductionDate,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };

        return new InductionPayment()
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = induction.ParticipantId,
            SupportWorker = induction.CreatedBy!,
            ContractId = induction.Location!.Contract!.Id,
            Approved = induction.Created!.Value,
            Induction = induction.InductionDate,
            LocationId = induction.LocationId,
            LocationType = induction.Location.LocationType.Name,
            TenantId = induction.Owner!.Tenant!.Id,
            EligibleForPayment = false,
            IneligibilityReason = ineligibilityReason.Value,
            PaymentPeriod = dates.Max(),
            CommencedDate = induction.InductionDate,
            InductionInput = induction.Created!.Value,
            CreatedOn = DateTime.UtcNow
        };
    }
}