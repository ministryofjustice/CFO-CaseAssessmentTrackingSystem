﻿using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PersonalDetail : BaseEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PersonalDetail()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string? PreferredNames { get; private set; }
    public string? PreferredPronouns { get; private set; }

    public static PersonalDetail CreateFrom(string? preferredNames, string? preferredPronouns)
    {
        PersonalDetail pd = new()
        {
            PreferredNames = preferredNames,
            PreferredPronouns = preferredPronouns
        };

        return pd;
    }
}
