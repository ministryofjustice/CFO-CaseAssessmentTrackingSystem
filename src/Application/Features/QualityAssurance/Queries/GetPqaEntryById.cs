﻿using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetPqaEntryById
{
    [RequestAuthorize(Policy = SecurityPolicies.Pqa)]
    public class Query : IRequest<Result<EnrolmentQueueEntryDto>>
    {
        public Guid Id { get; set; }
        public UserProfile? CurrentUser { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<EnrolmentQueueEntryDto>>
    {
        public async Task<Result<EnrolmentQueueEntryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var entry = await unitOfWork.DbContext.EnrolmentPqaQueue
                .Where(a => a.Id == request.Id && a.TenantId.StartsWith(request.CurrentUser!.TenantId!))
                .ProjectTo<EnrolmentQueueEntryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (entry == null)
            {
                //question: do we return a specific error if the entry exists but is at a different tenant?
                return Result<EnrolmentQueueEntryDto>.Failure("Not found");
            }

            return entry;

        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Id)
                .NotEmpty()
                .WithMessage(string.Format(ValidationConstants.GuidMessage, "Id"));
        }
    }
}
