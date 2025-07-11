namespace Cfo.Cats.Application.Common.Validators;

public static class ValidationConstants
{
    public const string LettersSpacesUnderscores = @"^[A-Za-z_ ]+$";
    public const string LettersSpacesUnderscoresMessage = "{0} must contain only letters, spaces, and underscores.";

    public const string LettersSpacesCommaApostrophe = @"^[A-Za-z ',’]+$";
    public const string LettersSpacesCommaApostropheMessage = "{0} must contain only letters, spaces, comma and an apostrophe.";

    public const string AlphabetsDigitsSpaceSlashHyphenDot= @"^[a-zA-Z0-9\s\\\/\-.]+$";
    public const string AlphabetsDigitsSpaceSlashHyphenDotMessage = "{0} must contain only alphabet (both uppercase and lowercase), digit, space, backslash, forward slash, hyphen, and dot.";

    public const string DateMustBeInPast = "Date must be in the past.";
    public const string DateMustBeInFuture = "Date must be in the future.";

    public const string Notes = @"^[A-Za-z0-9 ?.,!""'\/$£&€\r\n\-\(\)@’:;%]*$";
    public const string NotesMessage = "{0} must contain only letters, numbers, spaces and common punctuation";

    public const int NotesLength = 1000;

    public const string Guid = @"^\{?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}\}?$";
    public const string GuidMessage = "{0} must contain a valid Guid";

    public const string Numbers = @"^[0-9]*$";
    public const string NumbersMessage = "{0} must contain only numbers";

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

    public const string TenantId = @"^(\d+(\.\d+)*\.)$";
    public const string TenantIdMessage = "Invalid format for Tenant Id";

    public const string TenantDomain = @"^@[a-z0-9]+(?:[-]?[a-z0-9]+)*(?:\.[a-z0-9]+(?:[-]?[a-z0-9]+)*)+$";
    public const string TenantDomainMessage = "Must be in the format '@example.com'";

    public const string NumberBetweenZeroAndTenWithQuarterIncrement = @"^(10(\.00?)?|[0-9](\.(00?|25|50?|75?))?|\.(00?|25|50?|75?)?)$";
    public const string NumberWithTwoDecimalPlaces = @"^[0-9]*\.?[0-9]{1,2}$";

    public static class RuleSet
    {
        /// <summary>
        /// Default rule set, note this must be "default" in lower case to work with
        /// the fluent validation context selection. 
        /// </summary>
        public const string Default = "default";
        
        /// <summary>
        /// Rules that should run as part of the Mediator pipeline.
        /// These rules probably touch the database and should not run
        /// in the UI 
        /// </summary>
        public const string MediatR = "MediatR";
    }

}