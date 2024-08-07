﻿using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.DTOs;

[Description("Tenants")]
public record TenantDto
{
    [Description("Tenant Id")]
    public required string Id { get; set; }

    [Description("Parent Id")]
    public string? ParentId { get; private set; }

    [Description("Tenant Name")]
    public string? Name { get; set; }

    [Description("Description")]
    public string? Description { get; set; }

    public string[] Domains { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Tenant, TenantDto>()
                .ForMember(tenantDto => tenantDto.Domains, 
                    options => options.MapFrom(tenant => tenant.Domains.Select(d => d.Domain)))
                .ForMember(tenantDto => tenantDto.ParentId, 
                    options => options.MapFrom(tenant => tenant.Id.Substring(0, tenant.Id.Length - 2)))
                .ReverseMap();
        }
    }
}
