using System.Text.Json;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class IntegrationEvents
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private bool _enabled;
    private bool _publishing;
    private Type? _selectedType;
    private string _json = string.Empty;
    private IReadOnlyList<Type> _eventTypes = [];

    protected override void OnInitialized()
    {
        _enabled = Config.GetValue<bool>("Features:IntegrationEventPublishing:Enabled");

        if (_enabled == false)
        {
            return;
        }

        _eventTypes = typeof(IntegrationEvent).Assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsClass: true }
                        && typeof(IntegrationEvent).IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToArray();
    }

    private void OnEventTypeChanged(Type type)
    {
        _selectedType = type;

        try
        {
            var sample = CreateSample(type);
            _json = JsonSerializer.Serialize(sample, type, SerializerOptions);
        }
        catch (Exception ex)
        {
            _json = string.Empty;
            Snackbar.Add($"Could not generate template: {ex.Message}", Severity.Error);
        }
    }

    private async Task Publish()
    {
        if (_selectedType is null || _enabled == false)
        {
            return;
        }

        _publishing = true;

        try
        {
            object? message;

            try
            {
                message = JsonSerializer.Deserialize(_json, _selectedType);
            }
            catch (JsonException ex)
            {
                Snackbar.Add($"Invalid JSON: {ex.Message}", Severity.Error);
                return;
            }

            if (message is null)
            {
                Snackbar.Add("The payload could not be deserialized.", Severity.Error);
                return;
            }

            var insert = typeof(OutboxExtensions)
                .GetMethod(nameof(OutboxExtensions.InsertOutboxMessage))!
                .MakeGenericMethod(_selectedType);

            await (Task)insert.Invoke(null, [Service.DbContext, message])!;
            await Service.SaveChangesAsync();

            Snackbar.Add($"Published {_selectedType.Name} to the outbox.", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to publish: {ex.Message}", Severity.Error);
        }
        finally
        {
            _publishing = false;
        }
    }

    private static object CreateSample(Type type)
    {
        var ctor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var args = ctor.GetParameters()
            .Select(p => SampleValue(p.ParameterType))
            .ToArray();

        return ctor.Invoke(args);
    }

    private static object? SampleValue(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        if (underlying == typeof(string))
        {
            return string.Empty;
        }

        if (underlying == typeof(Guid))
        {
            return Guid.Empty;
        }

        if (underlying == typeof(DateTime))
        {
            return DateTime.UtcNow;
        }

        if (underlying == typeof(DateTimeOffset))
        {
            return DateTimeOffset.UtcNow;
        }

        if (underlying == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(DateTime.UtcNow);
        }

        if (underlying == typeof(TimeOnly))
        {
            return TimeOnly.MinValue;
        }

        if (underlying.IsEnum)
        {
            return Enum.GetValues(underlying).GetValue(0);
        }

        if (underlying.IsValueType)
        {
            return Activator.CreateInstance(underlying);
        }

        return null;
    }
}
