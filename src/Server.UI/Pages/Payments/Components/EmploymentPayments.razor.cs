﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public partial class EmploymentPayments
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


        var query = from ep in unitOfWork.DbContext.EmploymentPayments
            join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
            join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
            join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
            join a in unitOfWork.DbContext.EmploymentActivities on ep.ActivityId equals a.Id
            where dd.TheMonth == Month && dd.TheYear == Year
            select new
            {
                ep.CreatedOn,
                a.CommencedOn,
                ep.ActivityApproved,
                ep.ParticipantId,
                ep.EligibleForPayment,
                ep.LocationType,
                Location = l.Name,
                Contract = c.Description,
                ContractId = c.Id,
                ep.IneligibilityReason,
                TenantId = c!.Tenant!.Id!,
                ParticipantName = a.Participant!.FirstName + " " + a.Participant!.LastName,
                ep.PaymentPeriod,
                a.EmploymentType,
                EmploymentCategory = a.Definition.Category.Name
            };

        query = Contract is null
            ? query.Where(q => q.TenantId.StartsWith(CurrentUser.TenantId!))
            : query.Where(q => q.ContractId == Contract.Id);

        Payments = await query.AsNoTracking()
            .Select(x => new RawData
            {
                CreatedOn = x.CreatedOn,
                CommencedOn = x.CommencedOn,
                ActivityApproved = x.ActivityApproved,
                ParticipantId = x.ParticipantId,
                EligibleForPayment = x.EligibleForPayment,
                Contract = x.Contract,
                Location = x.Location,
                LocationType = x.LocationType,
                ParticipantName = x.ParticipantName,
                PaymentPeriod = x.PaymentPeriod,
                IneligibilityReason = x.IneligibilityReason,
                EmploymentCategory = x.EmploymentCategory,
                EmploymentType = x.EmploymentType
            })
            .OrderBy(e => e.Contract)
            .ThenByDescending(e => e.CreatedOn)
            .ToArrayAsync();

        this._summaryData = Payments
            .Where(e => e.EligibleForPayment)
            .GroupBy(e => e.Contract)
            .Select(x => new SummaryDataModel
            {
                Contract = x.Key,
                Employments = x.Count(),
                EmploymentsTarget = TargetProvider.GetTarget(x.Key, Month, Year).Employment
            })
            .OrderBy(c => c.Contract)
            .ToList();

        _loading = false;
    }

    private string _searchString = "";
    private List<SummaryDataModel> _summaryData = [];

    private record RawData
    {
        public required DateTime CreatedOn { get; set; }
        public required DateTime CommencedOn { get; set; }
        public required DateTime ActivityApproved { get; set; }
        public required DateTime PaymentPeriod { get; set; }
        public required string Contract { get; set; } 
        public required string LocationType { get; set; }
        public required string Location { get; set; }
        public required string ParticipantId { get; set; } 
        public required bool EligibleForPayment { get; set; }
        public required string? IneligibilityReason { get; set; }
        public required string ParticipantName { get; set; }
        public required string EmploymentType { get; set; }
        public required string EmploymentCategory { get; set; }
    }

    private class SummaryDataModel
    {
        public required string Contract { get; set; }
        public required int Employments { get; set; }
        public required int EmploymentsTarget { get; set; }
        public decimal EmploymentsPercentage => Employments.CalculateCappedPercentage(EmploymentsTarget);
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