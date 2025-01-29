using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordPreReleaseSupportPayment(IUnitOfWork unitOfWork) : IConsumer<PRIAssignedIntegrationEvent>
{
    private static class IneligibilityReasons
    {
        public const string AlreadyPaid = "A wing induction has already been claimed under this contract";
        public const string NotYetApproved = "The enrolment for this participant has not yet been approved";
        public const string BeforeConsent = "This occurred before the consent date";
    }

    public async Task Consume(ConsumeContext<PRIAssignedIntegrationEvent> context)
    {
        var db = unitOfWork.DbContext;

        var query = from pri in db.PRIs
            join u in db.Users on pri.CreatedBy equals u.Id
            where pri.Id == context.Message.PRIId
            select new
            {
                pri.Id,
                pri.ParticipantId
            };

        await Task.CompletedTask;

    }
}