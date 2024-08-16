﻿namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A11() : SingleChoiceQuestion("Have carried an illegal weapon at some point in the past",
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