namespace Cfo.Cats.Application.Features.Dashboard.DTOs;

public class CaseSummaryDto
{

    public EnrolmentStatus GetEnrolmentStatus() => EnrolmentStatus.FromValue(EnrolmentStatusId);

    public required  string UserName { get; set; } 
    public required string TenantName { get; set; }
    public required string LocationName { get; set; }

    public required int EnrolmentStatusId
    {
        get;
        set;
    }

    public required int Count { get; set; } 

}
