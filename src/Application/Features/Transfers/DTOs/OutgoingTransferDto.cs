using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Transfers.DTOs;

public class OutgoingTransferDto
{
    public required string ParticipantId { get; set; }
    public string? FromContractId { get; set; }
    public Contract? ToContractId { get; set; }
    public required LocationDto FromLocationId { get; set; }
    public required LocationDto ToLocationId { get; set; }
    public required DateTime MoveOccured { get; set; }
    public required TransferLocationType TransferType { get; set; }

    class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ParticipantOutgoingTransferQueueEntry, OutgoingTransferDto>();
        }
    }
}
