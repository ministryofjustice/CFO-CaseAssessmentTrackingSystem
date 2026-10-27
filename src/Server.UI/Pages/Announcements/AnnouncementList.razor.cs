using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.Features.Announcements.Queries;
using Cfo.Cats.Domain.Announcements;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Announcements;

public partial class AnnouncementList
{
    [Parameter] public AnnouncementAudience Audience { get; set; }
    [Parameter] public bool Manage { get; set; }
    [Parameter] public bool CanViewInternal { get; set; }
    [Parameter] public bool CanPublishImmediately { get; set; }
    [CascadingParameter] private UserProfile CurrentUser { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private INotificationService NotificationService { get; set; } = null!;
    [Inject] private ILogger<AnnouncementList> Logger { get; set; } = null!;

    private AnnouncementDto[] _announcements = [];
    private string? _selectedType;
    private string[] AvailableTypes => _announcements
        .Select(a => DisplayedRevision(a)?.TypeName)
        .OfType<string>()
        .Distinct(StringComparer.Ordinal)
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToArray();
    private AnnouncementDto[] FilteredAnnouncements => _announcements
        .Where(a => string.IsNullOrEmpty(_selectedType) || DisplayedRevision(a)?.TypeName == _selectedType)
        .ToArray();
    private bool _busy;
    private string? _error;

    private AnnouncementRevisionDto? DisplayedRevision(AnnouncementDto announcement)
        => Manage ? announcement.Pending ?? announcement.Published : announcement.Published;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        _busy = true;
        _error = null;
        _announcements = [];
        try
        {
            IQuery<Result<AnnouncementDto[]>> query = Manage
                ? new GetAnnouncementsForManagement.Query()
                : new GetAnnouncements.Query { Audience = Audience };
            var result = await GetNewMediator().Send(query);
            if (result is { Succeeded: true, Data: not null })
            {
                _announcements = result.Data;
                if (_selectedType is not null && !AvailableTypes.Contains(_selectedType))
                {
                    _selectedType = null;
                }
            }
            else
            {
                _error = result.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load announcements.");
            _error = "An unexpected error occurred while loading announcements.";
        }
        finally
        {
            _busy = false;
        }
    }

    private bool CanApprove(AnnouncementDto announcement) => announcement.Pending is not null
        && (CanPublishImmediately || announcement.Pending.SubmittedBy != CurrentUser.UserId);

    private string AccountName(string userId)
    {
        var user = UserService.DataSource.SingleOrDefault(u => u.Id == userId);
        return user is null ? userId : $"{user.DisplayName} ({user.UserName})";
    }

    private async Task EditAsync(AnnouncementDto? announcement)
    {
        var typesResult = await GetNewMediator().Send(new GetAnnouncementTypes.Query());
        if (typesResult is not { Succeeded: true, Data: not null })
        {
            Snackbar.Add(typesResult.ErrorMessage, Severity.Error);
            return;
        }
        if (typesResult.Data.Length == 0)
        {
            Snackbar.Add("Add an announcement type before creating an announcement.", Severity.Warning);
            return;
        }
        var revision = announcement?.Pending ?? announcement?.Published;
        var types = typesResult.Data.Select(x => x.Name).ToArray();
        var currentTypeAvailable = revision is null || types.Contains(revision.TypeName);
        var model = new SaveAnnouncement.Command
        {
            Id = announcement?.Id,
            Version = announcement?.Version ?? Guid.Empty,
            TypeName = revision is null ? types.FirstOrDefault(x => x == "General") ?? types[0]
                : currentTypeAvailable ? revision.TypeName : string.Empty,
            Audience = revision?.Audience
                ?? (CanViewInternal ? AnnouncementAudience.InternalStaff : AnnouncementAudience.AllStaff),
            Title = revision?.Title ?? string.Empty,
            PostedBy = revision?.PostedBy ?? (string.IsNullOrWhiteSpace(CurrentUser.DisplayName) ? CurrentUser.UserName : CurrentUser.DisplayName),
            Description = revision?.Description ?? string.Empty,
            Links = revision?.Links.Select(x => new AnnouncementLinkDto { Url = x.Url, DisplayName = x.DisplayName }).ToList() ?? []
        };
        var parameters = new DialogParameters<EditAnnouncementDialog>
        {
            { x => x.Model, model },
            { x => x.Types, types },
            { x => x.TypeUnavailable, !currentTypeAvailable },
            { x => x.CanViewInternal, CanViewInternal },
            { x => x.CanPublishImmediately, CanPublishImmediately }
        };
        var options = new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.Medium, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<EditAnnouncementDialog>(
            announcement is null ? "Add announcement" : "Edit announcement", parameters, options);
        if (await dialog.Result is { Canceled: false })
        {
            NotificationService.Refresh();
            await LoadAsync();
        }
    }

    private async Task ManageTypesAsync()
    {
        var options = new DialogOptions { FullWidth = true, MaxWidth = MaxWidth.Medium, CloseOnEscapeKey = true };
        await DialogService.ShowAsync<ManageAnnouncementTypesDialog>("Manage announcement types", options);
    }

    private Task ApproveAsync(AnnouncementDto announcement)
        => DialogServiceHelper.ShowConfirmationDialog(
            "Approve announcement",
            "Publish the pending version and notify its audience?",
            () => RunCommandAsync(new ApproveAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }));

    private Task DeleteAsync(AnnouncementDto announcement)
        => DialogServiceHelper.ShowConfirmationDialog(
            "Delete announcement",
            "Delete this announcement? It will no longer be visible to staff.",
            () => RunCommandAsync(new DeleteAnnouncement.Command { Id = announcement.Id, Version = announcement.Version }));

    private async Task RunCommandAsync(ICommand<Result> command)
    {
        _busy = true;
        try
        {
            var result = await GetNewMediator().Send(command);
            if (result.Succeeded)
            {
                Snackbar.Add("Announcement updated.", Severity.Success);
                NotificationService.Refresh();
                await LoadAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _busy = false;
        }
    }
}
