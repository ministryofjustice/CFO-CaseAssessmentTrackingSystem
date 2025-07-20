using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Domain.Entities.Activities;

public abstract class ActivityWithTemplate : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected ActivityWithTemplate()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected ActivityWithTemplate(ActivityContext context) : base(context) { }

    
    // Foreign key property
    public Guid DocumentId { get; protected set; }

    public virtual Document? Document { get; protected set; }

    public abstract string DocumentLocation { get; }

    public ActivityWithTemplate AddTemplate(Document document)
    {
        Document = document;
        return this;
    }

}
