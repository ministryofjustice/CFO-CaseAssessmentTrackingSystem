using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class PathwayPlanReviews
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    [Inject]
    private IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    private bool _showOverdueOnly;
    private string? _keyword;

    // Sentinel-backed filter selections: 0 / null represent "All".
    private int _participantLocationId;
    private string? _participantLocationName;
    private int _reviewLocationId;
    private string? _reviewLocationName;
    private int? _reviewReason;

    private static IEnumerable<PathwayPlanReviewReason> ReviewReasonOptions =>
        new[] { PathwayPlanReviewReason.Default }
            .Concat(PathwayPlanReviewReason.List
                .Where(r => r != PathwayPlanReviewReason.Default)
                .OrderBy(r => r.Name));

    private static string ReviewReasonText(PathwayPlanReviewReason reason) =>
        reason == PathwayPlanReviewReason.Default ? "Not reviewed yet" : reason.Name;

    private string ReviewReasonLabel =>
        _reviewReason is { } value && PathwayPlanReviewReason.TryFromValue(value, out var reason)
            ? ReviewReasonText(reason)
            : "All Review Reasons";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cached = await SessionStorage.GetAsync<PathwayPlanReviewsSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            RestoreState(sd);
        }
    }

    private void RestoreState(PathwayPlanReviewsSessionData sd)
    {
        VisualMode = sd.VisualMode;
        _showOverdueOnly = sd.ShowOverdueOnly;
        _keyword = sd.Keyword;

        if (sd.ParticipantLocationId != 0 && sd.ParticipantLocationName is not null)
        {
            _participantLocationId = sd.ParticipantLocationId;
            _participantLocationName = sd.ParticipantLocationName;
        }

        if (sd.ReviewLocationId != 0 && sd.ReviewLocationName is not null)
        {
            _reviewLocationId = sd.ReviewLocationId;
            _reviewLocationName = sd.ReviewLocationName;
        }

        _reviewReason = sd.ReviewReason;

        if (CanFilter is false)
        {
            return;
        }

        OnTenantSelected(string.IsNullOrWhiteSpace(sd.TenantId) ? null : sd.TenantId);
        OnUserSelected(string.IsNullOrWhiteSpace(sd.UserId) ? null : sd.UserId);
    }

    private async Task OnSearchChanged(string? keyword)
    {
        _keyword = keyword;
        await SaveSessionState();
    }

    private async Task OnVisualModeChanged(bool visualMode)
    {
        VisualMode = visualMode;
        await SaveSessionState();
    }

    private async Task OnShowOverdueOnlyChanged(bool showOverdueOnly)
    {
        _showOverdueOnly = showOverdueOnly;
        await SaveSessionState();
    }

    private async Task ShowParticipantLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(
            CurrentUser,
            l => l.LocationType.IsHub == false,
            title: "Select a participant location",
            showAllOption: _participantLocationId != 0,
            allOptionLabel: "All Participant Locations");

        if (location is not null)
        {
            _participantLocationId = location.Id;
            _participantLocationName = location.Id == 0 ? null : location.Name;
            await SaveSessionState();
        }
    }

    private async Task ShowReviewLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(
            CurrentUser,
            title: "Select a review location",
            showAllOption: _reviewLocationId != 0,
            allOptionLabel: "All Review Locations");

        if (location is not null)
        {
            _reviewLocationId = location.Id;
            _reviewLocationName = location.Id == 0 ? null : location.Name;
            await SaveSessionState();
        }
    }

    private async Task OnReviewReasonChanged(int? reason)
    {
        _reviewReason = reason;
        await SaveSessionState();
    }

    private async Task OnTenantSelectedWithSave(string? tenantId)
    {
        OnTenantSelected(tenantId);
        await SaveSessionState();
    }

    private async Task OnUserSelectedWithSave(string? userId)
    {
        OnUserSelected(userId);
        await SaveSessionState();
    }

    private async Task ClearSearch()
    {
        _keyword = null;
        _showOverdueOnly = false;
        _participantLocationId = 0;
        _participantLocationName = null;
        _reviewLocationId = 0;
        _reviewLocationName = null;
        _reviewReason = null;

        if (CanFilter)
        {
            OnClearFilter();
        }

        await SaveSessionState();
    }

    private Task SaveSessionState()
        => SessionStorage.SetAsync(PathwayPlanReviewsSessionData.FromState(
            VisualMode,
            _showOverdueOnly,
            SelectedTenantId,
            SelectedUserId,
            _participantLocationId,
            _participantLocationName,
            _reviewLocationId,
            _reviewLocationName,
            _reviewReason,
            _keyword));
}
