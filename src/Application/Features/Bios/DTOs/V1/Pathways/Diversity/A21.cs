using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A21() : SingleChoiceQuestion("Have carried or used a knife or blade when committing crime", [Yes, No, NA])
{
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
}
