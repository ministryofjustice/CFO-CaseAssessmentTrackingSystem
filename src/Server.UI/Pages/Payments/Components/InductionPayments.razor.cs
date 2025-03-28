using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class InductionPayments
{
    private bool _loading = true;

    [Parameter, EditorRequired] public bool DataView { get; set; }

    [Parameter, EditorRequired] public int Month { get; set; }

    [Parameter, EditorRequired] public int Year { get; set; }

    [Parameter] public ContractDto? Contract { get; set; }

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private RawData[] Payments { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var unitOfWork = GetNewUnitOfWork();

        var query = from ip in unitOfWork.DbContext.InductionPayments
                    join dd in unitOfWork.DbContext.DateDimensions on ip.PaymentPeriod equals dd.TheDate
                    join c in unitOfWork.DbContext.Contracts on ip.ContractId equals c.Id
                    join l in unitOfWork.DbContext.Locations on ip.LocationId equals l.Id
                    join p in unitOfWork.DbContext.Participants on ip.ParticipantId equals p.Id
                    where dd.TheMonth == Month && dd.TheYear == Year
                    select new
                    {
                        ip.InductionInput,
                        ip.CommencedDate,
                        ip.Approved,
                        ip.ParticipantId,
                        ip.EligibleForPayment,
                        Contract = c.Description,
                        ContractId = c.Id,
                        ip.LocationType,
                        Location = l.Name,
                        ip.IneligibilityReason,
                        TenantId = c!.Tenant!.Id!,
                        ip.PaymentPeriod,
                        ip.Induction,
                        ParticipantName = p.FirstName + " " + p.LastName
                    };

        query = Contract is null
                ? query.Where(q => q.TenantId.StartsWith(CurrentUser.TenantId!))
                : query.Where(q => q.ContractId == Contract.Id);


        Payments = await query.AsNoTracking()
            .Select(x => new RawData
            {
                CreatedOn = x.InductionInput,
                InductionDate = x.Induction,
                CommencedOn = x.CommencedDate,
                Approved = x.Approved,
                PaymentPeriod = x.PaymentPeriod,
                ParticipantId = x.ParticipantId,
                EligibleForPayment = x.EligibleForPayment,
                Contract = x.Contract,
                Location = x.Location,
                LocationType = x.LocationType,
                IneligibilityReason = x.IneligibilityReason,
                ParticipantName = x.ParticipantName
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
                Wings = x.Count(g => g.LocationType == "Wing"),
                Hubs = x.Count(g => g.LocationType == "Hub"),
                WingsTarget = TargetProvider.GetTarget(x.Key, Month, Year).Wings,
                HubsTarget = TargetProvider.GetTarget(x.Key, Month, Year).Hubs,
            })
            .OrderBy(c => c.Contract)
            .ToList();

        _loading = false;

    }

    private string _searchString = "";
    private List<SummaryDataModel> SummaryData = [];

    private class RawData
    {
        public DateTime CreatedOn { get; set; }
        public DateTime CommencedOn { get; set; }
        public DateTime InductionDate { get; set; }
        public DateTime? Approved { get; set; }
        public DateTime? PaymentPeriod { get; set; }

        public string Contract { get; set; } = "";
        public string ParticipantId { get; set; } = "";
        public bool EligibleForPayment { get; set; }
        public string LocationType { get; set; } = "";
        public string Location { get; set; } = "";
        public string? IneligibilityReason { get; set; }
        public string ParticipantName { get; set; } = "";
    }

    private class SummaryDataModel
    {
        public required string Contract { get; set; }
        public int Wings { get; set; }
        public int WingsTarget { get; set; }
        public decimal WingsPercentage => Wings.CalculateCappedPercentage(WingsTarget);
        public int Hubs { get; set; }
        public int HubsTarget { get; set; }
        public decimal HubsPercentage => Hubs.CalculateCappedPercentage(HubsTarget);
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
