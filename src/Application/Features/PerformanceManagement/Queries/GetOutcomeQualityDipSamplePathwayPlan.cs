using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Queries;

public static class GetOutcomeQualityDipSamplePathwayPlan
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipChecks)]
    public class Query : IRequest<Result<ParticipantDipSamplePathwayPlanDto>>
    {
        public required string ParticipantId { get; init; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<ParticipantDipSamplePathwayPlanDto>>
    {
        public async Task<Result<ParticipantDipSamplePathwayPlanDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var db = unitOfWork.DbContext;
#nullable disable 
            var query = from pp in db.PathwayPlans
                where pp.ParticipantId == request.ParticipantId
                select new ParticipantDipSamplePathwayPlanDto
                {
                    ParticipantId = pp.ParticipantId,
                    Objectives = (
                        from obj in db.Objectives
                        join u in db.Users on obj.CreatedBy equals u.Id
                        join completedByGroup in db.Users on obj.CompletedBy equals completedByGroup.Id into completedByJoin
                        from completedBy in completedByJoin.DefaultIfEmpty()
                        where obj.PathwayPlanId == pp.Id
                        orderby obj.Index
                        select
                            new ParticipantDipSampleObjectiveDto
                            {
                                Created = obj.Created.Value,
                                CreatedBy = u.DisplayName,
                                Description = obj.Description,
                                Completed = obj.Completed,
                                Status = obj.CompletedStatus.Name,
                                Justification = obj.Justification,
                                CompletedBy = completedBy.DisplayName,
                                Index = obj.Index,
                                Tasks = (
                                    from t in db.ObjectiveTasks
                                    join u in db.Users on t.CreatedBy equals u.Id
                                    join completedByGroup in db.Users on t.CompletedBy equals completedByGroup.Id into completedByJoin
                                    from completedBy in completedByJoin.DefaultIfEmpty()
                                    where t.ObjectiveId == obj.Id
                                    orderby t.Index
                                    select new ParticipantDipSampleObjectiveTaskDto
                                    {
                                        Id = t.Id,
                                        Index =  t.Index,
                                        Description = t.Description,
                                        Due = t.Due,
                                        Status = t.CompletedStatus.Name,
                                        Justification = t.Justification,
                                        Created = t.Created.Value,
                                        Completed = t.Completed,
                                        CreatedBy = u.DisplayName,
                                        CompletedBy = completedBy.DisplayName,
                                    }
                                ).ToArray()
                            }
                    ).ToArray()
                };

            var plan = await query
                .AsNoTracking()
                .FirstAsync(cancellationToken);

            var activityQuery = from a in db.Activities                                
                    .Include(a => a.TookPlaceAtLocation)                
                join t in (
                    from p in db.PathwayPlans
                    where p.ParticipantId == request.ParticipantId
                    join o in db.Objectives on p.Id equals o.PathwayPlanId
                    join ot in db.ObjectiveTasks on o.Id equals ot.ObjectiveId
                    select ot.Id
                ) on a.TaskId equals t
                join u in db.Users on a.CreatedBy equals u.Id
                orderby a.Created
                select new
                {
                    a, 
                    u.DisplayName
                };

            var activities = await activityQuery
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            var dtos = (from a in activities
                    
                       select a.a switch
                             {
                                 NonISWActivity n => (ParticipantDipSampleActivityDto)new ParticipantDipSampleNonIswActivityDto()
                                 {
                                     TaskId = n.TaskId, 
                                     AdditionalInformation = n.AdditionalInformation,
                                     ApprovedOn = n.CompletedOn,
                                     Category = n.Category,
                                     CommencedOn = n.CommencedOn,
                                     Created = n.Created.Value,
                                     CreatedBy = a.DisplayName,
                                     Definition = n.Definition,
                                     Location = n.TookPlaceAtLocation.Name,
                                     Status = n.Status,
                                     Type = n.Type
                                 },
                                 EducationTrainingActivity eta => new ParticipantDipSampleEducationAndTrainingActivityDto()
                                 {
                                     TaskId = eta.TaskId,
                                     AdditionalInformation = eta.AdditionalInformation,
                                     ApprovedOn = eta.CompletedOn,
                                     Category = eta.Category,
                                     CommencedOn = eta.CommencedOn,
                                     Created = eta.Created.Value,
                                     CreatedBy = a.DisplayName,
                                     Definition = eta.Definition,
                                     Location = eta.TookPlaceAtLocation.Name,
                                     Status = eta.Status,
                                     Type = eta.Type,
                                     CompletionStatus = eta.CourseCompletionStatus,
                                     CourseCommencedOn = eta.CourseCommencedOn,
                                     CourseCompletedOn = eta.CourseCompletedOn,
                                     CourseLevel = eta.CourseLevel,
                                     CourseTitle = eta.CourseTitle,
                                     CourseUrl = eta.CourseUrl,
                                     DocumentId = eta.DocumentId
                                 },
                                 ISWActivity isw => new ParticipantDipSampleIswActivityDto()
                                 {
                                     TaskId = isw.TaskId,
                                     AdditionalInformation = isw.AdditionalInformation,
                                     ApprovedOn = isw.CompletedOn,
                                     Category = isw.Category,
                                     CommencedOn = isw.CommencedOn,
                                     Created = isw.Created.Value,
                                     CreatedBy = a.DisplayName,
                                     Definition = isw.Definition,
                                     Location = isw.TookPlaceAtLocation.Name,
                                     Status = isw.Status,
                                     Type = isw.Type,
                                     BaselineAchievedOn = isw.BaselineAchievedOn,
                                     DocumentId = isw.DocumentId,
                                     HoursPerformedDuring = isw.HoursPerformedDuring,
                                     HoursPerformedPost = isw.HoursPerformedPost,
                                     HoursPerformedPre = isw.HoursPerformedPre,
                                     WraparoundSupportStartedOn = isw.WraparoundSupportStartedOn
                                 },
                                 EmploymentActivity ea => new ParticipantDipSampleEmploymentActivityDto()
                                 {
                                     TaskId = ea.TaskId,
                                     AdditionalInformation = ea.AdditionalInformation,
                                     ApprovedOn = ea.CompletedOn,
                                     Category = ea.Category,
                                     CommencedOn = ea.CommencedOn,
                                     Created = ea.Created.Value,
                                     CreatedBy = a.DisplayName,
                                     Definition = ea.Definition,
                                     Location = ea.TookPlaceAtLocation.Name,
                                     Status = ea.Status,
                                     Type = ea.Type,
                                     DocumentId = ea.DocumentId,
                                     EmployerName = ea.EmployerName,
                                     EmploymentCommenced = ea.EmploymentCommenced,
                                     EmploymentType = ea.EmploymentType,
                                     JobTitle = ea.JobTitle,
                                     JobTitleCode = ea.JobTitleCode,
                                     Salary = ea.Salary,
                                     SalaryFrequency = ea.SalaryFrequency
                                 },
                                 _ => throw new ApplicationException("Unknown activity type")
                             }).ToArray();

            foreach (var t in plan.Objectives.SelectMany(p => p.Tasks))
            {
                // get all the matching activities and project them into this
                t.Activities = dtos.Where(a => a.TaskId == t.Id).ToArray();
            }

#nullable restore
            return plan;

        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}