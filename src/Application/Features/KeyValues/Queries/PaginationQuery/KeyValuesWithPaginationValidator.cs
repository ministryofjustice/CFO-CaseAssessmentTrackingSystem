using Cfo.Cats.Application.Common.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery
{
    public class KeyValuesWithPaginationValidator: AbstractValidator<KeyValuesWithPaginationQuery>
    {
        public KeyValuesWithPaginationValidator()
        {
            RuleFor(r => r.Keyword)
                .Matches(RegularExpressionValidation.Keyword)
                .WithMessage(string.Format(RegularExpressionValidation.KeywordMessage, "Search Keyword"));

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

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(RegularExpressionValidation.AlphaNumeric)
                .WithMessage(string.Format(RegularExpressionValidation.AlphaNumericMessage, "OrderBy"));

        }
    }
}
