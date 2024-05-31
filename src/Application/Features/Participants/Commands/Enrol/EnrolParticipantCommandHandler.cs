using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Candidates;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Enrol;

public class EnrolParticipantCommandHandler : IRequestHandler<EnrolParticipantCommand, Result<string>>
{
    private readonly IApplicationDbContext dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly IMapper mapper;

    public EnrolParticipantCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
        this.mapper = mapper;
    }
    public async Task<Result<string>> Handle(EnrolParticipantCommand request, CancellationToken cancellationToken)
    {
        Candidate candidate = await dbContext.Candidates.FirstAsync(c => c.Id == request.Identifier, cancellationToken);
        Participant participant = Participant.CreateFrom(candidate, request.ReferralSource!, request.ReferralComments);
        participant.AssignTo(currentUserService.UserId);
        
        dbContext.Participants.Add(participant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return request.Identifier!;
    }
}