﻿namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A5() : SingleChoiceQuestion("A member of a Gypsy or Irish traveller community",
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