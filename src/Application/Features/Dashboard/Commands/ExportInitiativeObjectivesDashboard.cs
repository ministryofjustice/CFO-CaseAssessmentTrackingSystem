using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Dashboard.Commands;

public static class ExportInitiativeObjectivesDashboard
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required InitiativeObjectivesDashboardExportRequest Request { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService,
        IUserService userService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Request);

            var filename = ExportDocumentNaming.BuildFileName("InitiativeObjectivesDashboard");

            var tenantName = string.IsNullOrWhiteSpace(request.Request.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Request.TenantId)?.Name;

            var userName = string.IsNullOrWhiteSpace(request.Request.UserId)
                ? null
                : userService.GetDisplayName(request.Request.UserId);

            var description = ExportDocumentNaming.BuildDescription(
                "Initiative Objectives Dashboard Export",
                ("Tenant", tenantName),
                ("User", userName),
                ("Initiative", request.Request.InitiativeCode),
                ("Active Only", request.Request.ShowActiveOnly ? "Yes" : null));

            var document = GeneratedDocument.Create(
                DocumentTemplate.InitiativeObjectivesDashboard,
                filename,
                description,
                currentUser.UserId!,
                currentUser.TenantId!,
                json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IApplicationSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IApplicationSettings settings)
        {
            _currentUserService = currentUserService;
            _settings = settings;
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(_settings).Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(_settings);

            var hasRecentlyRequestedDocument = _unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == _currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }

    public class InitiativeObjectivesDashboardExportRequest
    {
        public string? UserId { get; init; }
        public string? TenantId { get; init; }
        public string? InitiativeCode { get; init; }
        public bool ShowActiveOnly { get; init; }
    }
}
