using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Transfers.DTOs;

public class OutgoingTransferDto
{
    public required Guid Id { get; set; }
    public string? FromContract { get; set; }
    public string? ToContract { get; set; }
    public required LocationDto FromLocation { get; set; }
    public required LocationDto ToLocation { get; set; }
    public required DateTime MoveOccured { get; set; }
    public required TransferLocationType TransferType { get; set; }

    public required string ParticipantId { get; set; }
    public required string ParticipantFullName { get; set; }

    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ParticipantOutgoingTransferQueueEntry, OutgoingTransferDto>()
                .ForMember(t => t.ParticipantId, options => options.MapFrom(source => source.ParticipantId))
                .ForMember(t => t.ParticipantFullName, options => options.MapFrom(source => source.Participant!.FirstName + " " + source.Participant.LastName))
                .ForMember(p => p.FromContract, options => options.MapFrom(src => src.FromContract == null ? null : src.FromContract.Description))
                .ForMember(p => p.ToContract, options => options.MapFrom(src => src.ToContract == null ? null : src.ToContract.Description));
        }
    }
}
