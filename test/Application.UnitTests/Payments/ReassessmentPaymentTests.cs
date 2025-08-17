using Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using NUnit.Framework;
using Shouldly;
using System;

namespace Cfo.Cats.Application.UnitTests.Payments;

public class ReassessmentPaymentTests
{
    private static readonly DateTime dateOfConsent = DateTime.Today.AddMonths(-6);

    [Test]
    public void NoConsent_Should_NotBePaid()
    {
        var data = GetData() with { DateOfFirstConsent = null };

        var payment = RecordReassessmentPaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.NotYetApproved.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void PaymentMadeInLastTwoPaymentMonths_Should_NotBePaid()
    {
        var data = GetData() with 
        { 
            PreviouslyPaidAssessments = [ new(DateTime.Today.AddMonths(-2)) ] 
        };

        var payment = RecordReassessmentPaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.MaximumPaymentLimitReached.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void InitialAssessmentCompletedInLastTwoPaymentMonths_Should_NotBePaid()
    {
        var data = GetData() with
        {
            InitialAssessmentCompletedOn = DateTime.Today.AddDays(-14),
            PreviouslyPaidAssessments = []
        };

        var payment = RecordReassessmentPaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(IneligibilityReason.InitialAssessmentCompletedInLastTwoMonths.Value);
        payment.EligibleForPayment.ShouldBe(false);
    }

    [Test]
    public void ValidPayment_Should_BePaid()
    {
        var data = GetData();

        var payment = RecordReassessmentPaymentConsumer.GeneratePayment(data);

        payment.IneligibilityReason.ShouldBe(null);
        payment.EligibleForPayment.ShouldBe(true);
    }

    private static RecordReassessmentPaymentConsumer.Data GetData()
    {
        var today = DateTime.Today;

        return new RecordReassessmentPaymentConsumer.Data
        {
            AssessmentId = Guid.NewGuid(),
            Completed = today,
            Created = today.AddDays(-1),
            ParticipantId = "1AAA1111A",
            SupportWorker = Guid.NewGuid().ToString(),
            LocationId = 1,
            LocationType = "Custody",
            ContractId = "DUMMY-CONTRACT-001",
            TenantId = "1.",
            DateOfFirstConsent = DateOnly.FromDateTime(dateOfConsent),
            InitialAssessmentCompletedOn = dateOfConsent,
            PreviouslyPaidAssessments = [ new(today.AddMonths(-3)) ]
        };
    }
}
