using Cfo.Cats.Application.Common.Exports;
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

public static class ExportActivities
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required AllActivitiesWithPagination.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITenantService tenantService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("Activities");

            var tenantName = string.IsNullOrWhiteSpace(request.Query.TenantId)
                ? null
                : tenantService.DataSource.FirstOrDefault(t => t.Id == request.Query.TenantId)?.Name;

            var ownerName = string.IsNullOrWhiteSpace(request.Query.OwnerId)
                ? null
                : await unitOfWork.DbContext.Users
                    .Where(u => u.Id == request.Query.OwnerId)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(cancellationToken);

            var statusName = request.Query.Status.HasValue
                ? ActivityStatus.FromValue(request.Query.Status.Value).Name
                : null;

            var typeName = request.Query.TypeFilter.HasValue
                ? ActivityType.FromValue(request.Query.TypeFilter.Value).Name
                : null;

            var description = ExportDocumentNaming.BuildDescription(
                "Activities Export",
                ("Search", request.Query.Keyword),
                ("Tenant", tenantName),
                ("Owner", ownerName),
                ("Location", request.Query.LocationName),
                ("Status", statusName),
                ("Type", typeName),
                ("Returned Within Days", request.Query.ReturnedWithinDays?.ToString()),
                ("Approved Within Days", request.Query.ApprovedWithinDays?.ToString()));

            var document = GeneratedDocument
                .Create(DocumentTemplate.Activities, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;
        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(30);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {cooldown.Humanize()} between requesting documents.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - cooldown;

            var hasRecentlyRequestedDocument = unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}
