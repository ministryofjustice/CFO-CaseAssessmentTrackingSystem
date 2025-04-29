using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using NUnit.Framework;
using System;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Payments;

public class ThroughTheGatePaymentTests
{
    static readonly DateTime actualReleaseDate = DateTime.Today.AddDays(-7);
    static readonly DateTime dateOfConsent = DateTime.Today.AddDays(-21);
    static readonly DateTime priCreated = DateTime.Today.AddDays(-14);
    static readonly DateTime meetingAttendedOn = DateTime.Today.AddDays(-15);

    [Test]
    public void NoConsent_ShouldNot_BePaid()
    {
        var data = GetData() with { DateOfFirstConsent = null };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.NotYetApproved.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void TaskTwoIncompleteWithinFourWeeksOfActualRelease_ShouldNot_BePaid()
    {
        var data = GetData() with 
        { 
            Tasks = 
            [ 
                new() { IsMandatory = true, Index = 1, Completed = priCreated, CompletionStatus = CompletionStatus.Done },
                new() { IsMandatory = true, Index = 2, Completed = actualReleaseDate.AddDays(28).AddDays(1), CompletionStatus = CompletionStatus.Done },
            ] 
        };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.RequiredTasksNotCompletedInTime("4 weeks").Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void TaskTwoMarkedAsNotRequired_ShouldNot_BePaid()
    {
        var data = GetData() with
        {
            Tasks =
            [
                new() { IsMandatory = true, Index = 1, Completed = priCreated, CompletionStatus = CompletionStatus.Done },
                new() { IsMandatory = true, Index = 2, Completed = actualReleaseDate, CompletionStatus = CompletionStatus.NotRequired },
            ]
        };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.RequiredTasksNotCompleted.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void MeetingBeforeConsent_ShouldNot_BePaid()
    {
        var data = GetData() with { MeetingAttendedOn = DateOnly.FromDateTime(dateOfConsent.AddDays(-1)) };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.BeforeConsent.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void MultiplePayments_ShouldNot_BePaid()
    {
        var data = GetData() with { CountOfPayments = 1 };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.MaximumPaymentLimitReached.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void ParticipantReleasedToUnexpectedLocation_ShouldNot_BePaid()
    {
        var data = GetData() with { CommunityLocationId = 999 };

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.NeverInLocation.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void ValidPayment_Should_BePaid()
    {
        var data = GetData();

        var payment = RecordThroughTheGatePaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(null);
        payment.EligibleForPayment.ShouldBe(true);
    }

    static RecordThroughTheGatePaymentConsumer.Data GetData()
    {
        return new RecordThroughTheGatePaymentConsumer.Data
        {
            PriId = Guid.NewGuid(),
            ActivityInput = priCreated,
            DateOfFirstConsent = DateOnly.FromDateTime(dateOfConsent),
            ActualReleaseDate = DateOnly.FromDateTime(actualReleaseDate),
            Approved = priCreated.AddDays(1),
            CommunityLocationId = 1,
            ContractId = "DUMMY-CONTRACT-001",
            CountOfPayments = 0,
            CustodyLocationId = 0,
            Locations =
            [
                new() { LocationId = 0, From = actualReleaseDate.AddMonths(-1) } ,
                new() { LocationId = 1, From = actualReleaseDate } ,
            ],
            LocationType = "Community",
            SupportWorker = Guid.NewGuid().ToString(),
            MeetingAttendedOn = DateOnly.FromDateTime(meetingAttendedOn),
            ParticipantId = "1AAA1111A",
            Tasks =
            [
                new() { IsMandatory = true, Index = 1, Completed = priCreated, CompletionStatus = CompletionStatus.Done },
                new() { IsMandatory = true, Index = 2, Completed = priCreated.AddDays(1), CompletionStatus = CompletionStatus.Done },
            ],
            TenantId = "1."
        };
    }
}