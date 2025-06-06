﻿using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.AuditTrails.Specifications;

public class AuditTrailAdvancedFilter : PaginationFilter
{
    public Dictionary<string, object>? PrimaryKey { get; set; }

    public AuditType? AuditType { get; set; }
    public AuditTrailListView ListView { get; set; } = AuditTrailListView.All;
    public UserProfile? CurrentUser { get; set; }
}
