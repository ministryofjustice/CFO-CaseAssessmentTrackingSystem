using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.GetSystemAuditTrailsWithPagination;

public class AuditTrailQueryValidator : AbstractValidator<AuditTrailsWithPaginationQuery>
{
    public AuditTrailQueryValidator()
    {

        RuleFor(r => r.PageNumber)
            .GreaterThan(0)
            .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

        RuleFor(r => r.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000)
            .WithMessage(string.Format(ValidationConstants.MaximumPageSizeMessage, "Page Size"));

        RuleFor(r => r.SortDirection)
            .Matches(ValidationConstants.SortDirection)
            .WithMessage(ValidationConstants.SortDirectionMessage);

    }
}