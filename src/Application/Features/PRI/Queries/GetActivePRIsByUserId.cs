using AutoMapper;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.Features.PRI.DTOs;
using Cfo.Cats.Application.Features.PRI.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PRI.Queries;

public static class GetActivePRIsByUserId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    

    public class Query : PRIAdvancedFilter, IRequest<PaginatedData<PRIPaginationDto>>
    {
        public PRIAdvancedSpecification Specification => new(this);
    }



    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<PRIPaginationDto>>
    {
        public async Task<PaginatedData<PRIPaginationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var data = await unitOfWork.DbContext.PRIs
                .Where(x => x.AssignedTo == request.CurrentUser!.UserId || x.CreatedBy == request.CurrentUser!.UserId && x.IsCompleted == false)
                .OrderBy($"{request.OrderBy} {request.SortDirection}")            
                .ProjectToPaginatedDataAsync<Domain.Entities.PRIs.PRI, PRIPaginationDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);                                

            //var data = await unitOfWork.DbContext.Participants
            // .ProjectToPaginatedDataAsync<Participant, ParticipantPaginationDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);
            //return data;


            return data;
        }
    }


    //public class Query : IRequest<Result<IEnumerable<PRIDto>>>
    //{
    //    public required string UserId { get; set; }
    //}
    //class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<IEnumerable<PRIDto>>>
    //{
    //    public async Task<Result<IEnumerable<PRIDto>>> Handle(Query request, CancellationToken cancellationToken)
    //    {
    //        await Task.CompletedTask;

    //        var data = await unitOfWork.DbContext.PRIs
    //            .Where(x => x.AssignedTo == request.UserId || x.CreatedBy == request.UserId && x.IsCompleted == false)
    //            .ProjectTo<PRIDto>(mapper.ConfigurationProvider)
    //            .ToListAsync(cancellationToken);

    //        //var data = await unitOfWork.DbContext.Participants.OrderBy($"{request.OrderBy} {request.SortDirection}")
    //        // .ProjectToPaginatedDataAsync<Participant, ParticipantPaginationDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);
    //        //return data;


    //        return Result<IEnumerable<PRIDto>>.Success(data);
    //    }
    //}

    //public class Query : PRIAdvancedFilter, IRequest<PaginatedData<PRIDto>>
    //{

    //    public Query()
    //    {
    //        SortDirection = "Desc";
    //        OrderBy = "Created";
    //    }

    //    public PRIAdvancedSpecification Specification => new(this);
    //}


    //public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<PRIDto>>
    //{
    //    public async Task<PaginatedData<PRIDto>> Handle(Query request, CancellationToken cancellationToken)
    //    {
    //        var query = unitOfWork.DbContext
    //            .PRI
    //            .AnyAsync(e => e.CommunitySW == currentuser 
    //                        || e.CustSW == currentuser
    //                        && e.completedFlag== null
    //                        , cancellationToken)
    //            .AsNoTracking();

    //        var sortExpression = GetSortExpression(request);

    //        var ordered = request.SortDirection.Equals("Descending", StringComparison.CurrentCultureIgnoreCase)
    //            ? query.OrderByDescending(sortExpression)
    //            : query.OrderBy(sortExpression);

    //        var data = await ordered
    //            .ProjectToPaginatedDataAsync<PRI, PRIDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);

    //        return data;
    //    }
    //    private Expression<Func<PRI, object?>> GetSortExpression(Query request)
    //    {
    //        Expression<Func<PRI, object?>> sortExpression;
    //        switch (request.OrderBy)
    //        {
    //            case "ParticipantId":
    //                sortExpression = (x => x.Participant!.FirstName + ' ' + x.Participant.LastName);
    //                break;
    //            case "TenantId":
    //                sortExpression = (x => x.TenantId);
    //                break;
    //            case "Created":
    //                sortExpression = (x => x.Created!);
    //                break;
    //            case "SupportWorker":
    //                sortExpression = (x => x.Participant!.Owner!.DisplayName!);
    //                break;
    //            case "AssignedTo":
    //                sortExpression = (x => x.OwnerId == null ? null : x.Owner!.DisplayName);
    //                break;
    //            default:
    //                sortExpression = (x => x.Created!);
    //                break;
    //        }

    //        return sortExpression;
    //    } 


    //    IEnumerable<PRIDto> testPRIDtoList = new List<PRIDto>
    //{
    //    //new PRIDto
    //    //{
    //    //    PRIId= Guid.NewGuid(),
    //    //    ParticipantId = "1CFG1479H",
    //    //    ExpectedReleaseDate= new DateOnly(2025, 1, 8),
    //    //    ActualDateOfRelease = new DateTime(2025, 1, 8)
    //    //},
    //    //new PRIDto
    //    //{
    //    //    Id = Guid.NewGuid(),
    //    //    ParticipantId = "1CFG1553F",
    //    //    ExpectedDateOfRelease = new DateTime(2025, 2, 15),
    //    //    ActualDateOfRelease = null
    //    //},
    //    //new PRIDto
    //    //{
    //    //    Id = Guid.NewGuid(),
    //    //    ParticipantId = "1CFG8262O",
    //    //    ExpectedDateOfRelease = null,
    //    //    ActualDateOfRelease = new DateTime(2025, 3, 5)
    //    //}
    //    };
}