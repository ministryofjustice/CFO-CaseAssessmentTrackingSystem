using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Notifications.DTOs;
using Cfo.Cats.Application.Features.Notifications.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Notifications;

namespace Cfo.Cats.Application.Features.Notifications.Queries;

public static class NotificationsWithPaginationQuery
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : NotificationsAdvancedFilter, IRequest<PaginatedData<NotificationDto>>
    {
        public NotificationAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<Query, PaginatedData<NotificationDto>>
    {
        public async Task<PaginatedData<NotificationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Notifications
                .Where(n => n.ReadDate.HasValue == request.ShowReadNotifications)
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Notification, NotificationDto>(request.Specification, request.PageNumber,
                    request.PageSize, mapper.ConfigurationProvider, cancellationToken);
            

            return data;
        }
    }
}