﻿namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B11() : SingleChoiceQuestion("Enjoyed going to school",
[
    Yes,
    No,
    NA
])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};