using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class SupportAndReferral
{
    private readonly ApexChartOptions<SummaryDataModel> _options = new();
    private bool _loading = true;

    [Parameter, EditorRequired] public bool DataView { get; set; }

    [Parameter, EditorRequired] public int Month { get; set; }

    [Parameter, EditorRequired] public int Year { get; set; }

    [Parameter] public ContractDto? Contract { get; set; }

    [CascadingParameter] public UserProfile CurrentUser { get; set; } = default!;

    private RawData[] Payments { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var unitOfWork = GetNewUnitOfWork();

        var query = from ep in unitOfWork.DbContext.SupportAndReferralPayments
            join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
            join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
            join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
            join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
            where dd.TheMonth == Month && dd.TheYear == Year
            select new
            {
                ep.ActivityInput,
                ep.CreatedOn,
                ep.Approved,
                ep.ParticipantId,
                PaymentDate = ep.PaymentPeriod,
                ep.EligibleForPayment,
                ep.SupportType,
                Contract = c.Description,
                ContractId = c.Id,
                ep.LocationType,
                Location = l.Name,
                ep.IneligibilityReason,
                TenantId = c!.Tenant!.Id!,
                ParticipantName = p.FirstName + " " + p.LastName
            };

        query = Contract is null
            ? query.Where(q => q.TenantId.StartsWith(CurrentUser.TenantId!))
            : query.Where(q => q.ContractId == Contract.Id);


        Payments = await query.AsNoTracking()
            .Select(x => new RawData
            {
                CreatedOn = x.CreatedOn,
                Approved = x.Approved,
                ParticipantId = x.ParticipantId,
                EligibleForPayment = x.EligibleForPayment,
                Contract = x.Contract,
                SupportType = x.SupportType,
                Location = x.Location,
                LocationType = x.LocationType,
                IneligibilityReason = x.IneligibilityReason,
                ParticipantName = x.ParticipantName,
                PaymentPeriod = x.PaymentDate
            })
            .OrderBy(e => e.Contract)
            .ThenByDescending(e => e.CreatedOn)
            .ToArrayAsync();

        this.SummaryData = Payments
            .Where(e => e.EligibleForPayment)
            .GroupBy(e => e.Contract)
            .Select(x => new SummaryDataModel
            {
                Contract = x.Key,
                PreReleaseSupport = x.Count(g => g.SupportType == "Pre-Release Support"),
                PreReleaseSupportTarget = TargetProvider.GetTarget(x.Key, Month, Year).PreReleaseSupport,
                ThroughTheGateSupport = x.Count(g => g.SupportType == "Through the Gate"),
                ThroughTheGateSupportTarget = TargetProvider.GetTarget(x.Key, Month, Year).ThroughTheGate,
            })
            .OrderBy(c => c.Contract)
            .ToList();

        _loading = false;
    }

    private string _searchString = "";
    private List<SummaryDataModel> SummaryData = [];

    private class RawData
    {
        public required DateTime CreatedOn { get; set; }
        public required DateTime Approved { get; set; }
        public required string Contract { get; set; } = "";
        public required string ParticipantId { get; set; } = "";
        public required bool EligibleForPayment { get; set; }
        public required string SupportType { get; set; } = "";
        public required string LocationType { get; set; } = "";
        public required string Location { get; set; } = "";
        public required string? IneligibilityReason { get; set; }
        public required string ParticipantName { get; set; } = "";
        public required DateTime PaymentPeriod { get; set; }
    }

    private class SummaryDataModel
    {
        public required string Contract { get; set; }
        public int PreReleaseSupport { get; set; }
        public int PreReleaseSupportTarget { get; set; }

        public decimal PreReleaseSupportPercentage =>
            PreReleaseSupport.CalculateCappedPercentage(PreReleaseSupportTarget);

        public int ThroughTheGateSupport { get; set; }
        public int ThroughTheGateSupportTarget { get; set; }

        public decimal ThroughTheGateSupportPercentage =>
            ThroughTheGateSupport.CalculateCappedPercentage(ThroughTheGateSupportTarget);
    }

    private bool FilterFunc1(RawData data) => FilterFunc(data, _searchString);

    private bool FilterFunc(RawData data, string searchString)
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

        if (data.IneligibilityReason is not null &&
            data.IneligibilityReason.Contains(searchString, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (data.SupportType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
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