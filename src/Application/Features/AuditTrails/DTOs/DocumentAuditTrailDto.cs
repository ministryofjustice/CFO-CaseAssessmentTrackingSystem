using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.AuditTrails.DTOs;

public class DocumentAuditTrailDto
{
    public required Guid DocumentId { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string DocumentTitle { get; set; }
    public required string DocumentDescription { get; set; }
    public required DocumentAuditTrailRequestType RequestType { get; set; }
    public required DateTime OccurredOn { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<DocumentAuditTrail, DocumentAuditTrailDto>(MemberList.None)
                .ForMember(dest => dest.DocumentTitle, src => src.MapFrom(opts => opts.Document!.Title))
                .ForMember(dest => dest.DocumentDescription, src => src.MapFrom(opts => opts.Document!.Description))
                .ForMember(dest => dest.UserName, src => src.MapFrom(opts => opts.User!.Email));
        }
    }
}
