using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Specifications;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
public class QAEnrolmentsResultsAdvancedSpecification : Specification<Participant>
{
    public QAEnrolmentsResultsAdvancedSpecification(QAEnrolmentsResultsAdvancedFilter filter)
    {
        Query.Where(e => e.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                .Where(e => e.EnrolmentLocation.Id == filter.Location!.Id, filter.Location is not null)
                .Where(e => e.EnrolmentStatus == filter.Status, filter.Status is not null)
                .Where(e => e.OwnerId == filter.UserProfile.UserId, filter.JustMyParticipants)
                .Where(e => e.Owner.TenantId.StartsWith(filter.UserProfile.TenantId!));

        //Make sure Enrolling ones are top of the list
        Query.OrderByDescending(e => e.Created)
                .ThenBy(e => e.LastModified);
    }
}