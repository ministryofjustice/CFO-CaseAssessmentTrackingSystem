using System.ComponentModel.DataAnnotations;

namespace Cfo.Cats.Infrastructure.Configurations;

public class DatabaseSettings : IValidatableObject
{
    /// <summary>
    ///     Database key constraint
    /// </summary>
    public const string Key = nameof(DatabaseSettings);

    /// <summary>
    ///     Represents the database provider, which to connect to
    /// </summary>
    public string DbProvider { get; set; } = string.Empty;

    /// <summary>
    ///     The connection string being used to connect with the given database provider
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DbProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DbProvider)} is not configured",
                new[] { nameof(DbProvider) }
            );
        }

        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) }
            );
        }
    }
}
