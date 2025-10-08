using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.QualityAssurance.Specifications;

public class QAEnrolmentsResultsAdvancedFilter : PaginationFilter
{
    public required UserProfile UserProfile { get; set; }
    public LocationDto? Location { get; set; }
    public EnrolmentStatus? Status { get; set; }    
    public bool JustMyParticipants { get; set; }
    public bool IncludeInternalNotes { get; set; }
}

public enum QAEnrolmentsResultsListView
{
    [Description("Default")] Default = 0
}