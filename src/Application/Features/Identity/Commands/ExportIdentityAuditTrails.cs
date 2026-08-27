using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;

namespace Cfo.Cats.Application.Features.Identity.Commands;

public static class ExportIdentityAuditTrails
{
    [RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsRead)]
    public class Command : ICommand<Result>
    {
        public required IdentityAuditTrailsExportRequest Request { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ISerializer serializer)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var document = GeneratedDocument.Create(
                DocumentTemplate.IdentityAuditTrails,
                "User Audit Export.xlsx",
                "User Audit Export",
                currentUserService.UserId!,
                currentUserService.TenantId!,
                searchCriteria: serializer.Serialize(request));

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class IdentityAuditTrailsExportRequest
    {
        public IdentityActionType? IdentityActionType { get; init; }
        public IdentityAuditTrailListView ListView { get; init; }
        public string? UserName { get; init; }
        public string OrderBy { get; init; } = "Id";
        public string SortDirection { get; init; } = "Descending";
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(60);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {_cooldown.Humanize()} between requesting this export.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command command)
        {
            var cooldownPeriod = DateTime.UtcNow - _cooldown;

            return _unitOfWork.DbContext.GeneratedDocuments.Any(document =>
                document.CreatedBy == _currentUserService.UserId &&
                document.Created > cooldownPeriod &&
                document.Template == DocumentTemplate.IdentityAuditTrails) is false;
        }
    }
}