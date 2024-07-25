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
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));

            RuleFor(r => r.SortDirection)
                .Matches(ValidationConstants.SortDirection)
                .WithMessage(ValidationConstants.SortDirectionMessage);

        }
    }
}
