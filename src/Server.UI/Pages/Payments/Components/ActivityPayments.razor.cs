using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class ActivityPayments
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

        var query = from ep in unitOfWork.DbContext.ActivityPayments
            join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
            join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
            join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
            join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
            where dd.TheMonth == Month && dd.TheYear == Year
            select new
            {
                ep.ActivityInput,
                ep.CommencedDate,
                ep.ActivityApproved,
                PaymentDate = ep.PaymentPeriod,
                ep.ParticipantId,
                ep.EligibleForPayment,
                ep.ActivityCategory,
                ep.ActivityType,
                Contract = c.Description,
                ContractId = c.Id,
                ep.LocationType,
                Location = l.Name,
                ep.IneligibilityReason,
                ep.PaymentPeriod,
                TenantId = c!.Tenant!.Id!,
                ParticipantName = p.FirstName + " " + p.LastName
            };

        query = Contract is null
            ? query.Where(q => q.TenantId.StartsWith(CurrentUser.TenantId!))
            : query.Where(q => q.ContractId == Contract.Id);


        Payments = await query.AsNoTracking()
            .Select(x => new RawData
            {
                CreatedOn = x.ActivityInput,
                CommencedOn = x.CommencedDate,
                ActivityApproved = x.ActivityApproved,
                ParticipantId = x.ParticipantId,
                EligibleForPayment = x.EligibleForPayment,
                Contract = x.Contract,
                Category = x.ActivityCategory,
                Type = x.ActivityType,
                Location = x.Location,
                LocationType = x.LocationType,
                IneligibilityReason = x.IneligibilityReason,
                ParticipantName = x.ParticipantName,
                PaymentPeriod = x.PaymentPeriod
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
                SupportWork = x.Count(g => g.Type == ActivityType.SupportWork.Name),
                SupportWorkTarget = TargetProvider.GetTarget(x.Key, Month, Year).SupportWork,
                CommunityAndSocial = x.Count(g => g.Type == ActivityType.CommunityAndSocial.Name),
                CommunityAndSocialTarget = TargetProvider.GetTarget(x.Key, Month, Year).CommunityAndSocial,
                HumanCitizenship = x.Count(g => g.Type == ActivityType.HumanCitizenship.Name),
                HumanCitizenshipTarget = TargetProvider.GetTarget(x.Key, Month, Year).HumanCitizenship,
                ISWSupport = x.Count(g => g.Type == ActivityType.InterventionsAndServicesWraparoundSupport.Name),
                ISWSupportTarget = TargetProvider.GetTarget(x.Key, Month, Year).Interventions
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
        public DateTime ActivityApproved { get; set; }
        public DateTime PaymentPeriod { get; set; }
        public string Contract { get; set; } = "";
        public string ParticipantId { get; set; } = "";
        public bool EligibleForPayment { get; set; }
        public string Category { get; set; } = "";
        public string Type { get; set; } = "";
        public string LocationType { get; set; } = "";
        public string Location { get; set; } = "";
        public string? IneligibilityReason { get; set; }
        public string ParticipantName { get; set; } = "";
    }

    private class SummaryDataModel
    {
        public required string Contract { get; set; }
        public int SupportWork { get; set; }
        public int SupportWorkTarget { get; set; }
        public decimal SupportWorkPercentage => SupportWork.CalculateCappedPercentage(SupportWorkTarget);
        public int CommunityAndSocial { get; set; }
        public int CommunityAndSocialTarget { get; set; }

        public decimal CommunityAndSocialPercentage =>
            CommunityAndSocial.CalculateCappedPercentage(CommunityAndSocialTarget);

        public int HumanCitizenship { get; set; }
        public int HumanCitizenshipTarget { get; set; }
        public decimal HumanCitizenshipPercentage => HumanCitizenship.CalculateCappedPercentage(HumanCitizenshipTarget);
        public int ISWSupport { get; set; }
        public int ISWSupportTarget { get; set; }
        public decimal ISWSupportPercentage => ISWSupport.CalculateCappedPercentage(ISWSupportTarget);
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

        if (data.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase))
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