using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Server.UI.Components;

public abstract class CatsComponent<TData> : OwningComponentBase<IMediator>
    where TData : class
{
    [Inject]
    protected ILogger<CatsComponent<TData>> Logger { get; set; } = null!;
    
    /// <summary>
    /// Current user profile from cascading parameter
    /// </summary>
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    /// <summary>
    /// Loading state for the component
    /// </summary>
    protected bool Loading = true;
    
    /// <summary>
    /// Error message to display if query fails
    /// </summary>
    protected string? ErrorMessage;

    /// <summary>
    /// The data loaded by the component
    /// </summary>
    protected TData? Data;

    /// <summary>
    /// Creates the query to execute for loading data
    /// </summary>
    protected abstract IRequest<Result<TData>> CreateQuery();

    /// <summary>
    /// Called when data is successfully loaded. Override for custom processing.
    /// </summary>
    /// <param name="data">The loaded data</param>
    protected virtual void OnDataLoaded(TData data) => Data = data;

    /// <summary>
    /// Called when query fails. Override for custom error handling.
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    protected virtual void OnError(string errorMessage) => ErrorMessage = errorMessage;

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    /// <summary>
    /// Loads data using the query. Can be called to refresh data.
    /// </summary>
    protected async Task<bool> LoadDataAsync()
    {
        try
        {
            Loading = true;
            var query = CreateQuery();
            var result = await Service.Send(query);

            if (IsDisposed)
            {
                return false;
            }

            if (result is { Data: not null, Succeeded: true })
            {
                OnDataLoaded(result.Data);
                ErrorMessage = null;
                return true;
            }
            else
            {
                // this is an assumption that the error message is suitable
                // for display.
                OnError(result.ErrorMessage);
                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception has made it's way to the UI thread.");
            OnError("An unexpected error has occurred");
            return false;
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Refreshes the component data
    /// </summary>
    public Task RefreshAsync() => LoadDataAsync();
    
}
