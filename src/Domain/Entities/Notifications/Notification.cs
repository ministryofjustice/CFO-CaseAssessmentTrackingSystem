using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Notifications;

public class Notification : OwnerPropertyEntity<Guid>
{
    #pragma warning disable CS8618
    private Notification()
    {
    }
    #pragma warning restore CS8618
    
    private Notification(string heading, string details, string userId)
    {
        this.Id = Guid.NewGuid();
        this.Heading = heading;
        this.Details = details;
        this.OwnerId = userId;

        this.AddDomainEvent(new NotificationCreatedDomainEvent(this));
    }

    public static Notification Create(string heading, string details, string userId)
    {
        return new Notification(heading, details, userId);
    }

    public Notification SetLink(string url)
    {
        this.Link = url;
        return this;
    }
    public Notification UpdateReadDate(DateTime? readDate)
    {
        ReadDate = readDate;
        return this;
    }

    public string Heading { get; private set; }
    public string Details { get; private set; }

    public string? Link {get; private set; }

    public DateTime? ReadDate { get; private set; }
}