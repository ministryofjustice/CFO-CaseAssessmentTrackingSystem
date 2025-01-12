using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Contracts.DTOs
{
    public class ContractDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string TenantId { get; set; }

        private class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<Contract, ContractDto>()
                    .ForMember(target => target.Name, options => options.MapFrom(source => source.Description));
            }
        }
    }
}
