namespace Cfo.Cats.Domain.Entities.Activities;

public class NonISWActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    NonISWActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    NonISWActivity(ActivityContext context) : base(context) { }

    public static NonISWActivity Create(ActivityContext context)
    {
        NonISWActivity activity = new(context);

        return activity;
    }
}
