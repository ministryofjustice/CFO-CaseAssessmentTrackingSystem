using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Payments.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Payments.Commands;

public static class ExportActivityPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Command : IRequest<Result>
    { 
        public required GetActivityPayments.Query Query { get; set; }
    }

    class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var document = GeneratedDocument
                .Create(DocumentTemplate.ActivityPayments, "ActivityPayments.xlsx", "ActivityPayments Export", currentUser.UserId!, currentUser.TenantId!, json);

            await unitOfWork.DbContext.Documents.AddAsync(document, cancellationToken);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly ICurrentUserService currentUserService;
        readonly IUnitOfWork unitOfWork;
        readonly TimeSpan cooldown = TimeSpan.FromSeconds(30);

        public Validator(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c)
                    .Must(WaitBeforeRequestingDocumentAgain)
                    .WithMessage($"You must wait {cooldown.Humanize()} between requesting documents.");
            });
        }

        bool WaitBeforeRequestingDocumentAgain(Command c)
        {
            var cooldownPeriod = DateTime.UtcNow - cooldown;

            var hasRecentlyRequestedDocument = unitOfWork.DbContext.GeneratedDocuments
                .Any(d => d.CreatedBy == currentUserService.UserId && d.Created > cooldownPeriod);

            return hasRecentlyRequestedDocument is false;
        }
    }
}