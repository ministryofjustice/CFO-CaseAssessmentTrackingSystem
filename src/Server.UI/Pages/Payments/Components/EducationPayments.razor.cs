using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class EducationPayments
{
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


        var query = from ep in unitOfWork.DbContext.EducationPayments
            join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
            join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
            join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
            join a in unitOfWork.DbContext.Activities on ep.ActivityId equals a.Id
            where dd.TheMonth == Month && dd.TheYear == Year
            select new
            {
                ep.ActivityInput,
                a.CommencedOn,
                ep.ActivityApproved,
                ep.ParticipantId,
                ep.EligibleForPayment,
                Contract = c.Description,
                ep.LocationType,
                ContractId = c.Id,
                Location = l.Name,
                ep.IneligibilityReason,
                TenantId = c!.Tenant!.Id!,
                ep.PaymentPeriod,
                ParticipantName = a.Participant!.FirstName + " " + a.Participant!.LastName
            };

        query = Contract is null
            ? query.Where(q => q.TenantId.StartsWith(CurrentUser.TenantId!))
            : query.Where(q => q.ContractId == Contract.Id);

        Payments = await query.AsNoTracking()
            .Select(x => new RawData
            {
                CreatedOn = x.ActivityInput,
                CommencedOn = x.CommencedOn,
                ActivityApproved = x.ActivityApproved,
                ParticipantId = x.ParticipantId,
                EligibleForPayment = x.EligibleForPayment,
                Contract = x.Contract,
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
                Educations = x.Count(),
                EducationsTarget = TargetProvider.GetTarget(x.Key, Month, Year).TrainingAndEducation
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
        public string LocationType { get; set; } = "";
        public string Location { get; set; } = "";
        public string? IneligibilityReason { get; set; }
        public string ParticipantName { get; set; } = "";
    }

    private class SummaryDataModel
    {
        public required string Contract { get; set; }
        public required int Educations { get; set; }
        public required int EducationsTarget { get; set; }
        public decimal EducationsPercentage => Educations.CalculateCappedPercentage(EducationsTarget);
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