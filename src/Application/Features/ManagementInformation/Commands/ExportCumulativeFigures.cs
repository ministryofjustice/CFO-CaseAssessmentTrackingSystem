using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Interfaces.Serialization;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands;

public static class ExportCumulativeFigures
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Command : ICommand<Result>
    {
        public DateOnly EndDate { get; init; }
        public string? ContractId { get; init; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ISerializer serializer,
        IContractService contractService)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var filename = ExportDocumentNaming.BuildFileName("CumulativeFigures");

            var contractName = string.IsNullOrWhiteSpace(request.ContractId)
                ? null
                : contractService.DataSource.FirstOrDefault(c => c.Id == request.ContractId)?.Name;

            var description = ExportDocumentNaming.BuildDescription(
                "Cumulative Figures",
                ("Contract", contractName),
                ("Date", request.EndDate.ToString("MMM yyyy")));

            var document = GeneratedDocument
                .Create(DocumentTemplate.CumulativeFigures, 
                    filename, 
                    description,
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
        private readonly IApplicationSettings _settings;

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IApplicationSettings settings)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _settings = settings;

            RuleSet(ValidationConstants.RuleSet.Mediator, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {ExportDocumentNaming.GetDocumentExportCooldown(_settings).Humanize()} between requesting this export.");
            });
        }

        private bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - ExportDocumentNaming.GetDocumentExportCooldown(_settings);

            var hasRecentlyRequestedDocument = _unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == _currentUserService.UserId && d.Created > cooldownPeriod
                    && d.Template == DocumentTemplate.CumulativeFigures);

            return hasRecentlyRequestedDocument is false;
        }
    }
}