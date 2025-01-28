using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands
{
    public static class AddActualReleaseDate
    {
        [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

        public class Command : IRequest<Result>
        {
            [Description("Participant Id")]
            public required string ParticipantId { get; set; }

            [Description("Actual Release Date")]
            public required DateTime? ActualReleaseDate { get; set; }         
        }

        public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {               
                var pri = await unitOfWork.DbContext.PRIs
                    .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId 
                    && p.ActualReleaseDate == null 
                    && p.IsCompleted==false, cancellationToken);

                if (pri == null)
                {
                    throw new NotFoundException("Cannot find PRI", request.ParticipantId);
                }

                pri.SetActualReleaseDate(DateOnly.FromDateTime((DateTime)request.ActualReleaseDate!));
                
                return Result.Success();
            }
        }
    }
}