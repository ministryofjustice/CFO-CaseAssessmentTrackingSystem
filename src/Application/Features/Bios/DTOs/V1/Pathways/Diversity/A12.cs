﻿namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A12() : SingleChoiceQuestion("Have undertaken prostitution or sex work",
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