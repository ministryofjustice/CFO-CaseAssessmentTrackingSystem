using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Payments.Commands;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class InductionPayments
{
    private bool _loading = true;
    private bool _downloading;

    [Parameter, EditorRequired] public bool DataView { get; set; }

    [Parameter, EditorRequired] public int Month { get; set; }

    [Parameter, EditorRequired] public int Year { get; set; }

    [Parameter] public ContractDto? Contract { get; set; }

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private InductionPaymentDto[] Payments { get; set; } = [];
    private List<InductionPaymentSummaryDto> SummaryData = [];

    GetInductionPayments.Query? Query;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;

            Query = new()
            {
                ContractId = Contract?.Id,
                Month = Month,
                Year = Year,
                TenantId = CurrentUser!.TenantId!
            };

            var mediator = GetNewMediator();

            var result = await mediator.Send(Query);

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
    private bool FilterFunc1(InductionPaymentDto data) => FilterFunc(data, _searchString);

    private bool FilterFunc(InductionPaymentDto data, string searchString)
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

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await GetNewMediator().Send(new ExportInductionPayments.Command()
            {
                Query = Query!
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);

        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }
}
