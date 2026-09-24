using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class ExportActivePRIs
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Command : ICommand<Result>
    {
        public required ActivePRIsExportRequest Request { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ILocationService locationService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Request);

            var filename = ExportDocumentNaming.BuildFileName("ActivePRIs");

            var custodySupportWorkerName = string.IsNullOrWhiteSpace(request.Request.CustodySupportWorker)
                ? null
                : await unitOfWork.DbContext.Users
                    .Where(u => u.Id == request.Request.CustodySupportWorker)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(cancellationToken);

            var communitySupportWorkerName = string.IsNullOrWhiteSpace(request.Request.CommunitySupportWorker)
                ? null
                : await unitOfWork.DbContext.Users
                    .Where(u => u.Id == request.Request.CommunitySupportWorker)
                    .Select(u => u.DisplayName)
                    .FirstOrDefaultAsync(cancellationToken);

            var expectedReleaseRegionName = request.Request.ExpectedReleaseRegionId.HasValue
                ? locationService.DataSource.FirstOrDefault(l => l.Id == request.Request.ExpectedReleaseRegionId.Value)?.Name
                : null;

            var description = ExportDocumentNaming.BuildDescription(
                "Active PRIs Export",
                ("Search", request.Request.Keyword),
                ("Just My PRIs", request.Request.JustMyPris ? "Yes" : null),
                ("Include Outgoing", request.Request.IncludeOutgoing ? "Yes" : null),
                ("Include Incoming", request.Request.IncludeIncoming ? "Yes" : null),
                ("Custody Support Worker", custodySupportWorkerName),
                ("Community Support Worker", communitySupportWorkerName),
                ("Expected Release Region", expectedReleaseRegionName),
                ("Active Status", request.Request.ActiveStatus.HasValue ? (request.Request.ActiveStatus.Value ? "Yes" : "No") : null));

            var document = GeneratedDocument
                .Create(DocumentTemplate.ActivePRIs, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

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

    public class ActivePRIsExportRequest
    {
        public string? UserId { get; init; }
        public string? Keyword { get; init; }
        public bool JustMyPris { get; init; }
        public bool IncludeOutgoing { get; init; }
        public bool IncludeIncoming { get; init; }
        public string? CustodySupportWorker { get; init; }
        public string? CommunitySupportWorker { get; init; }
        public int? ExpectedReleaseRegionId { get; init; }
        public bool? ActiveStatus { get; init; }
        public string? OrderBy { get; init; }
        public string? SortDirection { get; init; }
    }
}
