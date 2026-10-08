using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Workspaces.Performance.Commands;

public static class ExportEmployments
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required EmploymentExportRequest Request { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService,
        ILocationService locationService,
        IUserService userService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Request);

            var filename = ExportDocumentNaming.BuildFileName("PerformanceEmployments");

            var tenantName = string.IsNullOrWhiteSpace(request.Request.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Request.TenantId)?.Name;

            var userName = string.IsNullOrWhiteSpace(request.Request.UserId)
                ? null
                : userService.GetDisplayName(request.Request.UserId);

            var locationName = request.Request.LocationId.HasValue
                ? locationService.DataSource.FirstOrDefault(l => l.Id == request.Request.LocationId.Value)?.Name
                : null;

            var description = ExportDocumentNaming.BuildDescription(
                "Performance Employments Export",
                ("Tenant", tenantName),
                ("User", userName),
                ("Location", locationName),
                ("Location Type", request.Request.LocationType),
                ("Start Date", request.Request.StartDate.ToString("dd MMM yyyy")),
                ("End Date", request.Request.EndDate.ToString("dd MMM yyyy")));

            var document = GeneratedDocument.Create(
                DocumentTemplate.PerformanceEmployments,
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

    public class EmploymentExportRequest
    {
        public required DateTime StartDate { get; init; }
        public required DateTime EndDate { get; init; }
        public string? TenantId { get; init; }
        public string? UserId { get; init; }
        public int? LocationId { get; init; }
        public string? LocationType { get; init; }
    }
}
