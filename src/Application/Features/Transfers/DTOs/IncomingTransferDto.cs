using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Transfers.DTOs;

public class IncomingTransferDto
{
    public required Guid Id { get; set; }


    public required string ParticipantFullName { get; set; }
    public required string ParticipantId { get; set; }

    //public required ParticipantDto Participant { get; set; }
    public string? FromContract { get; set; }
    public string? ToContract { get; set; }
    public required LocationDto FromLocation { get; set; }
    public required LocationDto ToLocation { get; set; }
    public required DateTime MoveOccured { get; set; }
    public required TransferLocationType TransferType { get; set; }
    public required bool Completed { get; set; }

    class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ParticipantIncomingTransferQueueEntry, IncomingTransferDto>()
                .ForMember(p => p.ParticipantId, options => options.MapFrom(src => src.ParticipantId))
                .ForMember(p => p.ParticipantFullName, options => options.MapFrom(src => src.Participant!.FirstName + " " + src.Participant!.LastName))
                .ForMember(p => p.FromContract, options => options.MapFrom(src => src.FromContract == null ? null : src.FromContract.Description))
                .ForMember(p => p.ToContract, options => options.MapFrom(src => src.ToContract == null ? null : src.ToContract.Description));
        }
    }
}
