using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs
{
    #nullable disable
    public class EnrolmentQaNoteDto
    {
        public required DateTime Created {get; set;}
        public required string Message { get; set; }
        public required string CreatedBy { get; set; }
        public required string TenantName { get; set; }

        private class Mapper : Profile
        {
            
            public Mapper()
            {
                CreateMap<Note, EnrolmentQaNoteDto>()
                    .ForMember(target => target.CreatedBy, options => options.MapFrom(source=>source.CreatedByUser.DisplayName))
                    .ForMember(target => target.Message, options => options.MapFrom(source => source.Message))
                    .ForMember(target => target.Created, options => options.MapFrom(source => source.Created))
                    .ForMember(target => target.TenantName, options => options.MapFrom(source => source.CreatedByUser.TenantName));
            }


        }


    }
}