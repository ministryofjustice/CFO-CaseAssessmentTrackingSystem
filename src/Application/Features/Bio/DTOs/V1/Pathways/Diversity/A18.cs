﻿namespace Cfo.Cats.Application.Features.Bio.DTOs.V1.Pathways.Diversity;

public class A18() : SingleChoiceQuestion("Regret the offence(s) you committed",
[
    Yes,
    No,
    NA
])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "N/A";
};