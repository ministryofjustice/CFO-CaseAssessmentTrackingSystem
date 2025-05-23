﻿using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Payments;

public partial class Payments
{
    private bool _noAccessToContracts;
    
    [CascadingParameter] 
    public UserProfile? CurrentUser { get; set; }

    public int Month { get; set; } = DateTime.Now.Month;
    public int Year { get; set; } = DateTime.Now.Year;
    public bool DataView { get; set; } = false;
    public ContractDto? SelectedContract { get; set; }

    [Inject] private IContractService ContractService { get; set; } = default!;

    protected override void OnInitialized() => _noAccessToContracts = CurrentUser?.Contracts is [];
}