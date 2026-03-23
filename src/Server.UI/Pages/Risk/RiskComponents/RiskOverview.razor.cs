using Cfo.Cats.Application.Features.Participants.DTOs;
using FluentValidation.Internal;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;

public partial class RiskOverview
{
    [Parameter, EditorRequired]
    public required RiskDto Model { get; set; }

    [CascadingParameter]
    public MudForm? Form { get; set; }

    [Parameter, EditorRequired] 
    public RiskComponentViewMode ViewMode { get; set; }

    public bool ReadOnly { get; private set; } = false;

    protected override Task OnInitializedAsync()
    {
        if (Form is not null)
        {
            ReadOnly = Form.ReadOnly;
        }
        Model.IsRelevantToCustody = Model.IsRelevantToCustody || Model.LocationType?.IsCustody == true;
        Model.IsRelevantToCommunity = Model.IsRelevantToCommunity || Model.LocationType?.IsCommunity == true;

        return base.OnInitializedAsync();
    }

    public Action<ValidationStrategy<RiskDto>> Strategy => (options) =>
    {
        options.IncludeProperties(x => x.CommunityRiskDetail.RiskToChildren);
        options.IncludeProperties(x => x.CommunityRiskDetail.RiskToPublic);
        options.IncludeProperties(x => x.CommunityRiskDetail.RiskToKnownAdult);
        options.IncludeProperties(x => x.CommunityRiskDetail.RiskToStaff);
        options.IncludeProperties(x => x.CommunityRiskDetail.RiskToSelfNew);

        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToChildren);
        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToPublic);
        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToKnownAdult);
        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToStaff);
        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToOtherPrisoners);
        options.IncludeProperties(x => x.CustodyRiskDetail.RiskToSelfNew);

        options.IncludeProperties(x => x.IsRelevantToCommunity);
        options.IncludeProperties(x => x.IsRelevantToCustody);
    };

    public bool HasOptionSelected => Model.IsRelevantToCommunity || Model.IsRelevantToCustody;
}