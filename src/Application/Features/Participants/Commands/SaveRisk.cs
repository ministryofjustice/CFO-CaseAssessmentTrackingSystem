using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class SaveRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<Guid>>
    {
        public required Guid RiskId { get; set; }
        public required RiskDto Risk { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var risk = await unitOfWork.DbContext.Risks.FindAsync(request.RiskId);

            if(risk is null)
            {
                return Result<Guid>.Failure("Risk not found");
            }

            risk = mapper.Map(request.Risk, risk);

            unitOfWork.DbContext.Risks.Update(risk);

            return Result<Guid>.Success(risk.Id);
        }
    }

}
