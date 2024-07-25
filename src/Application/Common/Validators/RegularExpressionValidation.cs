using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Common.Validators
{
    public static class RegularExpressionValidation
    {
        public const string LettersSpacesUnderscores = @"^[A-Za-z_ ]+$";
        public const string LettersSpacesUnderscoresMessage = "{0} must contain only letters, spaces, and underscores.";

        public const string Guid = @"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$";
        public const string GuidMessage = "{0} must contain a valid Guid";

        public const string AlphaNumeric = @"^[a-zA-Z0-9]*$";
        public const string AlphaNumericMessage = "{0} must contain only alphabets and/or numbers.";
        
        public const string Keyword = @"^[a-zA-Z0-9_ \-\(\)@.,'’]*$";
        public const string KeywordMessage = "{0} must contain only alphabets, numbers, underscores, hyphen, brackets and spaces.";

        public const string NameCompliantWithDMS = @"^[\p{L}\s\-'’]+$";
        public const string NameCompliantWithDMSMessage = "{0} must contain only alphabets, spaces, ', ’ or -";
        
        public const string Date = @"^(?:([12][0-9]|0?[1-9])[\\/](0?2)|(30|[12][0-9]|0?[1-9])[\\/]([469]|11)|(3[01]|[12][0-9]|0?[1-9])[\\/](0?[13578]|1[02]))[\\/]((?:[0-9]{2})?[0-9]{2})$";
        public const string DateMessage = @"Date must be in dd/mm/yyyy or dd\mm\yyyy format, dd and mm may contain 1 or 2 digit and year must contain 4 digit";

        public const string SortDirection = @"^(?i)(ascending|descending|asc|desc)$";
        public const string SortDirectionMessage = "Invalid value for sort order";

        public static string PositiveNumberMessage = "{0} must be positive number and greater than 0.";

        public static string PageSizeMessage = "Page size must be between 1 and 1000.";
    }
}
