using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class ExportTasksDue
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required TasksDueExportRequest Request { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Request);

            var filename = ExportDocumentNaming.BuildFileName("TasksDue");

            var tenantName = string.IsNullOrWhiteSpace(request.Request.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Request.TenantId)?.Name;

            var supportWorkerName = string.IsNullOrWhiteSpace(request.Request.UserId)
                ? null
                : await unitOfWork.DbContext.Users
                    .Where(u => u.Id == request.Request.UserId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(cancellationToken);

            var description = ExportDocumentNaming.BuildDescription(
                "Tasks Due Export",
                ("Search", request.Request.Keyword),
                ("Tenant", tenantName),
                ("Support Worker", supportWorkerName));

            var document = GeneratedDocument
                .Create(DocumentTemplate.TasksDue, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(30);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {_cooldown.Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - _cooldown;

            var hasRecentlyRequestedDocument = _unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == _currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }

    public class TasksDueExportRequest
    {
        public string? UserId { get; init; }
        public string? OwnerId { get; init; }
        public string? TenantId { get; init; }
        public TaskDueCategory? Category { get; init; }
        public string? Keyword { get; init; }
        public string? OrderBy { get; init; }
        public string? SortDirection { get; init; }
    }
}
