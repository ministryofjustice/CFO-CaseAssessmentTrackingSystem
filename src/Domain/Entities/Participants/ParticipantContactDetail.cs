using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantContactDetail : BaseEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantContactDetail()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static ParticipantContactDetail Create(
        string participantId, 
        string description, 
        string address, 
        string postCode, 
        string uprn, 
        string? mobileNumber = null,
        string? emailAddress = null)
    {
        var contact = new ParticipantContactDetail
        {
            ParticipantId = participantId,
            Description = description,
            Address = address,
            PostCode = postCode,
            UPRN = uprn,
            MobileNumber = mobileNumber,
            EmailAddress = emailAddress
        };

        //contact.AddDomainEvent(new ParticipantContactDetailAddedDomainEvent(this));

        return contact;
    }

    public ParticipantContactDetail SetPrimary(bool primary)
    {
        Primary = primary;
        return this;
    }

    public string ParticipantId { get; private set; }

    /// <summary>
    /// A description of the contact. E.g. Sibling's contact details.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Indicates whether this contact is the primary point of contact for the participant.
    /// </summary>
    public bool Primary { get; private set; }

    /// <summary>
    /// The geographical address. E.g. 1 High Street, London, England, AA1 1AA.
    /// </summary>
    public string Address { get; private set; }

    /// <summary>
    /// The postcode of the address.
    /// </summary>
    public string PostCode { get; private set; }

    /// <summary>
    /// The Unique Property Referene Number (UPRN) of the address.
    /// </summary>
    public string UPRN { get; private set; }

    /// <summary>
    /// Mobile phone number of the registered contact.
    /// </summary>
    public string? MobileNumber { get; private set; }

    /// <summary>
    /// Email Address of the registered contact.
    /// </summary>
    public string? EmailAddress { get; private set; }
}
