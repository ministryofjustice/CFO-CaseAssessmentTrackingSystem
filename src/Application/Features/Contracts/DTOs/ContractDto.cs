using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Contracts.DTOs;

public class ContractDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string TenantId { get; set; }

    private class Mapper : Profile
    {
        public Mapper() =>
            CreateMap<Contract, ContractDto>()
                .ForMember(t => t.Id, o => o.MapFrom(s => s.Id))
                .ForMember(t => t.Name, o => o.MapFrom(s => s.Description))
                .ForMember(t => t.TenantId, o => o.MapFrom(s => s.Tenant!.Id));
    }
}
