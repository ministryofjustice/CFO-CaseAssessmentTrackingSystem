using Cfo.Cats.Application.Common.Validators;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.Export
{
    public class ExportKeyValuesQueryValidator: AbstractValidator<ExportKeyValuesQuery>
    {
        public ExportKeyValuesQueryValidator()
        {

            RuleFor(r => r.Keyword)
                .Matches(RegularExpressionValidation.Keyword)
                .WithMessage(string.Format(RegularExpressionValidation.KeywordMessage, "Search Keyword"));

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(RegularExpressionValidation.AlphaNumeric)
                .WithMessage(string.Format(RegularExpressionValidation.AlphaNumericMessage, "OrderBy"));

            RuleFor(r => r.SortDirection)
                .Matches(RegularExpressionValidation.SortDirection)
                .WithMessage(RegularExpressionValidation.SortDirectionMessage);

        }
    }
}
