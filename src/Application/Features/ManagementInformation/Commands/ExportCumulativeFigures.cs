using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands;

public static class ExportCumulativeFigures
{
    [RequestAuthorize(Roles = $"{RoleNames.Finance}, {RoleNames.SystemSupport}")]
    public class Command : IRequest<Result>
    {
        public DateOnly EndDate { get; init; }
        public string? ContractId { get; init; }
    }

    private class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ISerializer serializer)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var document = GeneratedDocument
                .Create(DocumentTemplate.CumulativeFigures, 
                    "Cumulative Figures.xlsx", 
                    "Cumulative Figures",
                    currentUserService.UserId!,
                    currentUserService.TenantId!,
                    searchCriteria: serializer.Serialize(request));
            
            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
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

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {_cooldown.Humanize()} between requesting this export.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - _cooldown;

            var hasRecentlyRequestedDocument = _unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == _currentUserService.UserId && d.Created > cooldownPeriod
                    && d.Template == DocumentTemplate.CumulativeFigures);

            return hasRecentlyRequestedDocument is false;
        }
    }
}