using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Participants.Rules;

public class ParticipantMustBeArchivedToDismissTransferRule(EnrolmentStatus enrolmentStatus) : IBusinessRule
{
    public bool IsBroken() => enrolmentStatus != EnrolmentStatus.ArchivedStatus;

    public string Message => "A transfer can only be dismissed when the participant is in the Archived status.";
}
