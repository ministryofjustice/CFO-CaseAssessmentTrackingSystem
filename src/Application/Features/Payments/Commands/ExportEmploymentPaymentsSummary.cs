using Cfo.Cats.Application.Common.Exports;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Payments.Commands;

public static class ExportEmploymentPaymentsSummary
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Command : ICommand<Result>
    {
        public required GetEmploymentPayments.Query Query { get; set; }
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IContractService contractService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var filename = ExportDocumentNaming.BuildFileName("EmploymentPaymentsSummary");

            var contractName = string.IsNullOrWhiteSpace(request.Query.ContractId)
                ? null
                : contractService.DataSource.FirstOrDefault(c => c.Id == request.Query.ContractId)?.Name;

            var description = ExportDocumentNaming.BuildDescription(
                "EmploymentPaymentsSummary Export",
                ("Contract", contractName),
                ("Date", new DateTime(request.Query.Year, request.Query.Month, 1).ToString("MMM yyyy")));

            var document = GeneratedDocument
                .Create(DocumentTemplate.EmploymentPaymentsSummary, filename, description, currentUser.UserId!, currentUser.TenantId!, json);

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
}
