using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.AuditTrails.Queries.PaginationQuery
{
    public class AuditTrailQueryValidator : AbstractValidator<AuditTrailsWithPaginationQuery>
    {
        public AuditTrailQueryValidator()
        {

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(RegularExpressionValidation.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage(string.Format(RegularExpressionValidation.PageSizeMessage, "Page Size"));

            RuleFor(r => r.SortDirection)
                .Matches(RegularExpressionValidation.SortDirection)
                .WithMessage(RegularExpressionValidation.SortDirectionMessage);

        }
    }
}
