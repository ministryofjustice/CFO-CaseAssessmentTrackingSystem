using Cfo.Cats.Application.Features.Tenants.DTOs;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Export;

public class ExportTenantsQueryHandler(IUnitOfWork unitOfWork, IExcelService excelService, IStringLocalizer<ExportTenantsQueryHandler> localizer, IMapper mapper)
    : IRequestHandler<ExportTenantsQuery, Result<byte[]>>
{
    // The is only used to get the member name descriptions.
    private readonly TenantDto dto = new() { Id = "" };


    public async Task<Result<byte[]>> Handle(ExportTenantsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.Tenants.ApplySpecification(request.Specification)
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