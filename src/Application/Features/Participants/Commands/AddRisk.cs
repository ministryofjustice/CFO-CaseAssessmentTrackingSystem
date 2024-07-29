﻿
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class AddRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            Risk risk = Risk.CreateFrom(Guid.NewGuid(), request.ParticipantId);
            await unitOfWork.DbContext.Risks.AddAsync(risk, cancellationToken);
            return Result<Guid>.Success(risk.Id);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric);
        }
    }

}