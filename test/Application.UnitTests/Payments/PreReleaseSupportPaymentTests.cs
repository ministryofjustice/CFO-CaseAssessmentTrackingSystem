using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using NUnit.Framework;
using System;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Payments;

public class PreReleaseSupportPaymentTests
{
    static readonly DateTime dateOfConsent = DateTime.Today.AddDays(-21);
    static readonly DateTime meetingAttendedOn = DateTime.Today.AddDays(-15);

    [Test]
    public void NoConsent_Should_NotBePaid()
    {
        var data = GetData() with { DateOfFirstConsent = null };

        var payment = RecordPreReleaseSupportPayment.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.NotYetApproved.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void MultiplePayments_Should_NotBePaid()
    {
        var data = GetData() with { CountOfPayments = 1 };

        var payment = RecordPreReleaseSupportPayment.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.MaximumPaymentLimitReached.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void MeetingBeforeConsent_Should_NotBePaid()
    {
        var data = GetData() with { MeetingAttendedOn = DateOnly.FromDateTime(dateOfConsent.AddDays(-1)) };

        var payment = RecordPreReleaseSupportPayment.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.BeforeConsent.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void ValidPayment_Should_BePaid()
    {
        var data = GetData();

        var payment = RecordPreReleaseSupportPayment.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(null);
        payment.EligibleForPayment.ShouldBe(true);
    }

    static RecordPreReleaseSupportPayment.Data GetData()
    {
        var today = DateTime.Today;

        return new RecordPreReleaseSupportPayment.Data
        {
            PriId = Guid.NewGuid(),
            ActivityInput = today,
            Approved = today,
            ParticipantId = "1AAA1111A",
            SupportWorker = Guid.NewGuid().ToString(),
            LocationId = 1,
            LocationType = "Custody",
            ContractId = "DUMMY-CONTRACT-001",
            MeetingAttendedOn = DateOnly.FromDateTime(meetingAttendedOn),
            TenantId = "1.",
            DateOfFirstConsent = DateOnly.FromDateTime(dateOfConsent),
            CountOfPayments = 0
        };
    }
}