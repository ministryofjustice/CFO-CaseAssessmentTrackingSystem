using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class ExportParticipants
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required ParticipantsWithPagination.Query Query { get; set; }
    }


    class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request.Query);

            var document = GeneratedDocument
                .Create(DocumentTemplate.Participants, "Participants.xlsx", "Participants Export", currentUser.UserId!, currentUser.TenantId!, json)
                .WithStatus(DocumentStatus.Processing)
                .WithExpiry(DateTime.UtcNow.AddDays(7));

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

            RuleFor(c => c)
                .Must(WaitBeforeRequestingDocumentAgain)
                .WithMessage($"You must wait {cooldown.Humanize()} between requesting documents.");
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
