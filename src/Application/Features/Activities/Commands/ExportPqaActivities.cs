using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class ExportPqaActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Command : ICommand<Result>
    {
        public required ActivityPqaQueueWithPagination.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService,
        IUserService userService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("PqaActivities");

            var tenantName = string.IsNullOrWhiteSpace(request.Query.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Query.TenantId)?.Name;

            var supportWorkerName = string.IsNullOrWhiteSpace(request.Query.SupportWorkerId)
                ? null
                : userService.GetDisplayName(request.Query.SupportWorkerId);

            var activityTypeName = request.Query.ActivityTypeId.HasValue
                ? ActivityType.FromValue(request.Query.ActivityTypeId.Value).Name
                : null;

            var description = ExportDocumentNaming.BuildDescription(
                "PqaActivities Export",
                ("Search", request.Query.Keyword),
                ("Tenant", tenantName),
                ("Support Worker", supportWorkerName),
                ("Activity Type", activityTypeName));

            var document = GeneratedDocument
                .Create(DocumentTemplate.PqaActivities, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IApplicationSettings settings;
        private readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IApplicationSettings settings)
        {
            this.currentUserService = currentUserService;
            this.settings = settings;
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(settings).Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(settings);

            var hasRecentlyRequestedDocument = unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}