using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetEnrolmentQaNotes 
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<EnrolmentQaNoteDto[]>>
    {
        public string? ParticipantId { get;set; }

        public UserProfile? CurentUser {get;set;}
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<EnrolmentQaNoteDto[]>>
    {
        public async Task<Result<EnrolmentQaNoteDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var pqa = await GetPqaNotes(request.ParticipantId!);
            var qa1 = await GetQa1Notes(request.ParticipantId!);
            var qa2 = await GetQa2Notes(request.ParticipantId!);


            return Result<EnrolmentQaNoteDto[]>.Success(pqa.Union(qa1).Union(qa2).ToArray());

        }

        private async Task<EnrolmentQaNoteDto[]> GetPqaNotes(string participantId)
        {
            var query1 = unitOfWork.DbContext.EnrolmentPqaQueue
                                    .AsNoTracking()
                                    .Where(c => c.ParticipantId == participantId)
                                    .SelectMany(c => c.Notes)
                                    .ProjectTo<EnrolmentQaNoteDto>(mapper.ConfigurationProvider);


             var results = await query1.ToArrayAsync()!;
             return results;
        }

        private async Task<EnrolmentQaNoteDto[]> GetQa1Notes(string participantId)
        {
            var query1 = unitOfWork.DbContext.EnrolmentQa1Queue
                                    .AsNoTracking()
                                    .Where(c => c.ParticipantId == participantId)
                                    .SelectMany(c => c.Notes)
                                    .ProjectTo<EnrolmentQaNoteDto>(mapper.ConfigurationProvider);


             var results = await query1.ToArrayAsync()!;
             return results;
        }

          private async Task<EnrolmentQaNoteDto[]> GetQa2Notes(string participantId)
        {
            var query1 = unitOfWork.DbContext.EnrolmentQa2Queue
                                    .AsNoTracking()
                                    .Where(c => c.ParticipantId == participantId)
                                    .SelectMany(c => c.Notes)
                                    .ProjectTo<EnrolmentQaNoteDto>(mapper.ConfigurationProvider);


             var results = await query1.ToArrayAsync()!;
             return results;
        }

    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
            
            RuleFor(x => x.CurentUser)
                .NotNull();

        }        
    }
    
}