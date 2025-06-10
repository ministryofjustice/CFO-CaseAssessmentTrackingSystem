using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Delius.DTOs;

public class OffenderManagerSummaryDto
{
    public string? OrganisationCode { get; set; } = default;
    public string? OrganistationDescription { get; set; } = default;
    public string? TeamCode { get; set; } = default;
    public string? TeamDescription { get; set; } = default;

}
