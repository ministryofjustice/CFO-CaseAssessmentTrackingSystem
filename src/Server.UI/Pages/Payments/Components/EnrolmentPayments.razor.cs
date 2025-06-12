using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class EnrolmentPayments
{
    private bool _loading = true;

    [Parameter, EditorRequired]
    public bool DataView { get; set; }

    [Parameter, EditorRequired]
    public int Month { get; set; }

    [Parameter, EditorRequired]
    public int Year { get; set; }

    [Parameter]
    public ContractDto? Contract { get; set; }

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private EnrolmentPaymentDto[] Payments { get; set; } = [];
    private List<EnrolmentPaymentSummaryDto> SummaryData = [];

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;

            var mediator = GetNewMediator();

            var result = await mediator.Send(new GetEnrolmentPayments.Query()
            {
                ContractId = Contract?.Id,
                Month = Month,
                Year = Year,
                TenantId = CurrentUser!.TenantId!
            });

            if (result is not { Succeeded: true })
            {
                throw new Exception(result.ErrorMessage);
            }

            Payments = result.Data?.Items ?? [];
            SummaryData = result.Data?.ContractSummary ?? [];

        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { _loading = false; }
    }

    private string _searchString = "";

    private bool FilterFunc1(EnrolmentPaymentDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(EnrolmentPaymentDto data, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return true;
        }

        if (data.ParticipantName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (data.ParticipantId.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (data.IneligibilityReason is not null && data.IneligibilityReason.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (data.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (data.LocationType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

}
