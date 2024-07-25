namespace Cfo.Cats.Application.Common.Validators
{
    public static class ValidationConstants
    {
        public const string LettersSpacesUnderscores = @"^[A-Za-z_ ]+$";
        public const string LettersSpacesUnderscoresMessage = "{0} must contain only letters, spaces, and underscores.";

        public const string GuidMessage = "{0} must contain a valid Guid";

        public const string AlphaNumeric = @"^[a-zA-Z0-9]*$";
        public const string AlphaNumericMessage = "{0} must contain only alphanumeric characters";
        
        public const string Keyword = @"^[a-zA-Z0-9_ \-\(\)@.,'’]*$";
        public const string KeywordMessage = "{0} must contain only alphanumeric, underscores, hyphen, brackets and spaces.";

        public const string NameCompliantWithDMS = @"^[\p{L}\s\-'’]+$";
        public const string NameCompliantWithDMSMessage = "{0} must contain only alphanumeric, spaces, ', ’ or -";
      
        public const string SortDirection = @"^(?i)(ascending|descending|asc|desc)$";
        public const string SortDirectionMessage = "Invalid value for sort order";

        public const string PositiveNumberMessage = "{0} must be positive number and greater than 0.";

        public const int MaximumPageSize = 50;
        public const string MaximumPageSizeMessage = "Page size must be between 1 and 50.";
    }
}
