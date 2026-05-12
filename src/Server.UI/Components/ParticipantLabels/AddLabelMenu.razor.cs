using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.ParticipantLabels.AddParticipantLabel;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;
using Cfo.Cats.Server.UI.Pages.Candidates;
using System;

namespace Cfo.Cats.Server.UI.Components.ParticipantLabels;

public partial class AddLabelMenu
{

    private bool _showDialog = false;
    private bool _saving;

    private LabelDto? _selectedItem = null;

    private string? _searchText = null;

    /// <summary>
    /// The participant to whom we are adding the label to
    /// </summary>
    [Parameter, EditorRequired]
    public ParticipantId ParticipantId { get; set; }

    /// <summary>
    /// The labels the user has access to select
    /// </summary>
    [Parameter, EditorRequired]
    public LabelDto[] VisibleLabels { get; set; }

    /// <summary>
    /// The labels the participant already has associated with them
    /// </summary>
    [Parameter, EditorRequired]
    public Guid[] AlreadySelected { get; set; }

    /// <summary>
    /// Event that fires when a label has been added.
    /// </summary>
    [Parameter, EditorRequired]
    public EventCallback LabelAdded { get; set; }

    private async Task AddLabel(LabelDto? label)
    {
        try
        {
            _saving = true;
            if(label is not null)
            { 
                var response = await Service.Send(new AddParticipantLabelCommand(ParticipantId, new LabelId(label.Id)));
                if (!response.Succeeded)
                {
                    Snackbar.Add(response.ErrorMessage, Severity.Error);
                }
                else
                {
                    _selectedItem = null;
                    _showDialog = false;
                    await LabelAdded.InvokeAsync();
                }
            }
        }
        finally
        {
            _saving = false;
        }
        
    }

    private string SelectedRowClassFunc(LabelDto? label, int rowNumber) => 
        _selectedItem is not null && _selectedItem.Equals(label) ? "selected-table-row-primary" : string.Empty;

    private bool FilterFunc1(LabelDto label) => FilterFunc(label, _searchText);

    private bool FilterFunc(LabelDto label, string? searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if(label.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        if(label.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

}