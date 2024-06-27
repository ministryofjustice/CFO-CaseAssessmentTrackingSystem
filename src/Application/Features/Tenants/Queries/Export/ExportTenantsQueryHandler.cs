using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Export;

public class ExportTenantsQueryHandler 
    : IRequestHandler<ExportTenantsQuery, Result<byte[]>>
{
    private readonly IApplicationDbContext context;
    // The is only used to get the member name descriptions.
    private readonly TenantDto dto = new() { Id = "" };
    private readonly IExcelService excelService;
    private readonly IStringLocalizer<ExportTenantsQueryHandler> localizer;
    private readonly IMapper mapper;

    public ExportTenantsQueryHandler(IApplicationDbContext context, IExcelService excelService, IStringLocalizer<ExportTenantsQueryHandler> localizer, IMapper mapper)
    {
        this.context = context;
        this.excelService = excelService;
        this.localizer = localizer;
        this.mapper = mapper;
    }


    public async Task<Result<byte[]>> Handle(ExportTenantsQuery request, CancellationToken cancellationToken)
    {
        var data = await context.Tenants.ApplySpecification(request.Specification)
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectTo<TenantDto>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = await excelService.ExportAsync(data,
            new Dictionary<string, Func<TenantDto, object?>>
            {
                { localizer[dto.GetMemberDescription(x => x.Id)], item => item.Id },
                { localizer[dto.GetMemberDescription(x => x.Name)], item => item.Name },
                { localizer[dto.GetMemberDescription(x => x.Description)], item => item.Description }
            }
        );
        return await Result<byte[]>.SuccessAsync(result);
    }
}