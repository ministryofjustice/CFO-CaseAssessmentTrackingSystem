using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Transfers.DTOs;

public class IncomingTransferDto
{
    public required string ParticipantId { get; set; }
    public string? FromContractId { get; set; }
    public string? ToContractId { get; set; }
    public required int FromLocationId { get; set; }
    public required int ToLocationId { get; set; }
    public required DateTime MoveOccured { get; set; }
    public required TransferLocationType TransferType { get; set; }
    public required bool Completed { get; set; }

    class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ParticipantIncomingTransferQueueEntry, IncomingTransferDto>();
        }
    }
}
