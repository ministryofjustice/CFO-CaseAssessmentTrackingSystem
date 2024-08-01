namespace Cfo.Cats.Application.Common.Validators
{
    public static class ValidationConstants
    {
        public const string LettersSpacesUnderscores = @"^[A-Za-z_ ]+$";
        public const string LettersSpacesUnderscoresMessage = "{0} must contain only letters, spaces, and underscores.";

        public const string Notes = @"^[A-Za-z0-9 ?.,!""'\/$£&€\r\n\-\(\)@’;%]*$";
        public const string NotesMessage = "{0} must contain only letters, numbers, spaces and common punctuation";
        
        public const string GuidMessage = "{0} must contain a valid Guid";

        public const string AlphaNumeric = @"^[a-zA-Z0-9]*$";
        public const string AlphaNumericMessage = "{0} must contain only alphanumeric characters";
        
        public const string Keyword = @"^[a-zA-Z0-9_ \-\(\)@.,'’]*$";
        public const string KeywordMessage = "{0} must contain only alphanumeric, underscores, hyphen, brackets, spaces and common punctuation.";

        public const string NameCompliantWithDMS = @"^[\p{L}\s\-'’]+$";
        public const string NameCompliantWithDMSMessage = "{0} must contain only alphanumeric, spaces, ', ’ or -";
      
        public const string SortDirection = @"^(?i)(ascending|descending|asc|desc)$";
        public const string SortDirectionMessage = "Invalid value for sort order";

        public const string PositiveNumberMessage = "{0} must be positive number and greater than 0.";

        public const int MaximumPageSize = 50;
        public const string MaximumPageSizeMessage = "Page size must be between 1 and 50.";

        public const string TenantId = @"^[0-9]+(?:\\.[0-9]+)*\\.$";
        public const string TenantIdMessage = "Must start with a number and end with a period for example 1.2.1.";

        public const string TenantDomain = @"^@[a-z0-9]+(?:[-]?[a-z0-9]+)*(?:\.[a-z0-9]+(?:[-]?[a-z0-9]+)*)+$";
        public const string TenantDomainMessage = "Must be in the format '@example.com'";
        
    }
}
