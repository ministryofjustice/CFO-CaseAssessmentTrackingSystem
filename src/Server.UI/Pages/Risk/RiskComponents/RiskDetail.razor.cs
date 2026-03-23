using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class RiskDetail
{
    [Parameter, EditorRequired]
    public required RiskDto.RiskDetail Model { get; set; }
    [Parameter]
    public bool Community { get; set; } = false;

    [CascadingParameter]
    public MudForm? Form { get; set; }

    public bool ReadOnly { get; private set; } = false;

    protected override Task OnInitializedAsync()
    {
        if (Form is not null)
        {
            ReadOnly = Form.ReadOnly;
        }

        return base.OnInitializedAsync();
    }
}