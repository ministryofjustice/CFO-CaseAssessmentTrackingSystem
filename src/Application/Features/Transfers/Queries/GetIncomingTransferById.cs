using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Transfers.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Transfers.Queries;

public static class GetIncomingTransferById
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<IncomingTransferDto>>
    {
        public required Guid IncomingTransferId { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<IncomingTransferDto>>
    {
        public async Task<Result<IncomingTransferDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var incomingTransfer = await unitOfWork.DbContext.ParticipantIncomingTransferQueue
                .ProjectTo<IncomingTransferDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(t => t.Id == request.IncomingTransferId, cancellationToken);

            if(incomingTransfer is null)
            {
                return Result<IncomingTransferDto>.Failure();
            }

            return Result<IncomingTransferDto>.Success(incomingTransfer);
        }
    }
}
