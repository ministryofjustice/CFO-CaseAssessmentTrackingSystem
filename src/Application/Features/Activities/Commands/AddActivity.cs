using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.Domain.Entities.Documents;
using Humanizer.Bytes;
using Microsoft.AspNetCore.Components.Forms;

namespace Cfo.Cats.Application.Features.Activities.Commands;

public static class AddActivity
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public record class Command(string ParticipantId) : IRequest<Result>
    {
        public Guid? ActivityId { get; set; }
        public required Guid TaskId { get; set; }

        [Description("Location")]
        public LocationDto? Location { get; set; }

        [Description("Activity/ETE")]
        public ActivityDefinition? Definition { get; set; }

        [Description("Date activity took place")]
        public DateTime? CommencedOn { get; set; }

        [Description("Additional Information")]
        public string? AdditionalInformation { get; set; }

        public EmploymentDto EmploymentTemplate { get; set; } = new() { ParticipantId = ParticipantId };
        public EducationTrainingDto EducationTrainingTemplate { get; set; } = new() { ParticipantId = ParticipantId };
        public IswDto ISWTemplate { get; set; } = new() { ParticipantId = ParticipantId };

        [Description("Upload Template")]
        public IBrowserFile? Document { get; set; }

        public bool CanChangeLocation => ActivityId is null;
        public bool CanChangeActivityDefinition => ActivityId is null;

        class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Activity, Command>()
                    .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Document, opt => opt.Ignore())
                    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.TookPlaceAtLocation))
                    .ForPath(dest => dest.EmploymentTemplate, opt => opt.MapFrom(src => src as EmploymentActivity))
                    .ForPath(dest => dest.EducationTrainingTemplate, opt => opt.MapFrom(src => src as EducationTrainingActivity))
                    .ForPath(dest => dest.ISWTemplate, opt => opt.MapFrom(src => src as ISWActivity));

                // Self-mapping for updates
                CreateMap<Activity, Activity>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Created, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())
                    .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                    .ForMember(dest => dest.Owner, opt => opt.Ignore())
                    .ForMember(dest => dest.Participant, opt => opt.Ignore())
                    .AfterMap((_, destination) => destination.ClearDomainEvents());

                CreateMap<ActivityWithTemplate, ActivityWithTemplate>()
                    .IncludeBase<Activity, Activity>()
                    .ForMember(dest => dest.Document, opt =>
                    {
                        opt.PreCondition((source, destination, context) => source.Document is not null);
                        opt.MapFrom(source => source.Document);
                    });

                CreateMap<EducationTrainingActivity, EducationTrainingActivity>().IncludeBase<Activity, Activity>();
                CreateMap<EmploymentActivity, EmploymentActivity>().IncludeBase<Activity, Activity>();
                CreateMap<ISWActivity, ISWActivity>().IncludeBase<Activity, Activity>();
                CreateMap<NonISWActivity, NonISWActivity>().IncludeBase<Activity, Activity>();
            }
        }
    }

    class Handler(
        IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IUploadService uploadService,
        IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .Include(p => p.CurrentLocation)
                .ThenInclude(l => l.Contract)
                .FirstOrDefaultAsync(p => p.Id == request.ParticipantId, cancellationToken);

            if (participant is null)
            {
                return Result.Failure("Participant not found");
            }

            var location = await unitOfWork.DbContext.Locations
                .Include(l => l.Contract)
                .FirstOrDefaultAsync(l => l.Id == request.Location!.Id, cancellationToken);

            if (location is not { Contract: not null })
            {
                return Result.Failure("Activities cannot be recorded against the chosen location");
            }

            var task = await unitOfWork.DbContext.PathwayPlans
                .SelectMany(p => p.Objectives)
                .SelectMany(o => o.Tasks.Where(task => task.Id == request.TaskId))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (task is null)
            {
                return Result.Failure("Task not found");
            }

            var cxt = new Activity.ActivityContext(
                Definition: request.Definition!,
                ParticipantId: participant.Id,
                Task: task,
                TookPlaceAtLocation: location,
                TookPlaceAtContract: location.Contract,
                ParticipantCurrentLocation: participant.CurrentLocation,
                ParticipantCurrentContract: participant.CurrentLocation.Contract,
                ParticipantStatus: participant.EnrolmentStatus!,
                CommencedOn: request.CommencedOn!.Value,
                currentUserService.TenantId!,
                currentUserService.UserId!,
                AdditionalInformation: request.AdditionalInformation);

            var classification = request.Definition!.Classification;

            Activity activity = classification switch
            {
                _ when classification == ClassificationType.EducationAndTraining => EducationTrainingActivity.Create(cxt,
                    request.EducationTrainingTemplate!.CourseTitle!,
                    request.EducationTrainingTemplate!.CourseUrl,
                    request.EducationTrainingTemplate!.CourseLevel!,
                    request.EducationTrainingTemplate!.CourseCommencedOn!.Value,
                    request.EducationTrainingTemplate.CourseCompletedOn,
                    request.EducationTrainingTemplate!.CourseCompletionStatus!
                ),
                _ when classification == ClassificationType.Employment => EmploymentActivity.Create(cxt,
                    request.EmploymentTemplate!.EmploymentType!,
                    request.EmploymentTemplate!.EmployerName!,
                    request.EmploymentTemplate!.JobTitle!,
                    request.EmploymentTemplate!.JobTitleCode!,
                    request.EmploymentTemplate!.Salary,
                    request.EmploymentTemplate!.SalaryFrequency,
                    request.EmploymentTemplate!.EmploymentCommenced!.Value
                ),
                _ when classification == ClassificationType.ISWActivity => ISWActivity.Create(cxt,
                    request.ISWTemplate!.WraparoundSupportStartedOn!.Value,
                    request.ISWTemplate!.HoursPerformedPre,
                    request.ISWTemplate!.HoursPerformedDuring,
                    request.ISWTemplate!.HoursPerformedPost,
                    request.ISWTemplate!.BaselineAchievedOn!.Value
                ),
                _ => NonISWActivity.Create(cxt)
            };

            // Upload template (if required)
            if (activity is ActivityWithTemplate templatedActivity && request.Document is not null)
            {
                var document = Document
                    .Create(request.Document.Name,
                            $"Activity Template for {request.ParticipantId}",
                            DocumentType.PDF);
                
                string path = $"{request.ParticipantId}/{templatedActivity.DocumentLocation}";

                long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes).Bytes);
                await using var stream = request.Document.OpenReadStream(maxSizeBytes);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, cancellationToken);

                var uploadRequest = new UploadRequest(request.Document.Name, UploadType.Document, memoryStream.ToArray());

                var result = await uploadService.UploadAsync(path, uploadRequest);

                if (result.Succeeded)
                {
                    document.SetURL(result);                 
                    await unitOfWork.DbContext.Documents.AddAsync(document);
                }                
                else                
                {
                    return Result.Failure("Failed to upload template");
                }

                templatedActivity.AddTemplate(document);
            }                        

            if (request.ActivityId is null)
            {
                await unitOfWork.DbContext.Activities.AddAsync(activity, cancellationToken);
            }
            else
            {
                var entity = await unitOfWork.DbContext.Activities
                    .Include(a => a.Owner)
                    .SingleAsync(a => a.Id == request.ActivityId, cancellationToken);

                mapper.Map(activity, entity);
                entity.TransitionTo(ActivityStatus.SubmittedToProviderStatus);
            }

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            When(c => c.ActivityId is not null, () =>
            {
                RuleFor(c => c.ActivityId)
                    .Must(BeInPendingStatus);

                RuleFor(c => c.Definition)
                    .Must((command, definition) => NotBeDifferentFromTheOriginal(command.ActivityId!.Value, definition!))
                    .When(c => c.Definition is not null)
                    .WithMessage("Changing activity type is not permitted");
            });

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9);

            RuleFor(c => c.Location)
                .NotNull()
                .WithMessage("You must choose a location");

            RuleFor(c => c.Location)
                .Must((command, location, token) => HaveAHubInduction(command.ParticipantId, location!))
                .When(c => c.Location is { LocationType.IsHub: true })
                .WithMessage("A hub induction is required for the selected location");

            When(c => c.Location is not null, () =>
            {
                RuleFor(c => c.Definition)
                    .NotNull()
                    .WithMessage("You must choose an Activity/ETE");
            });

            RuleFor(c => c.CommencedOn)
                .NotNull()
                .WithMessage("You must provide the date the activity took place")
                .Must(NotBeCompletedInTheFuture)
                .WithMessage("The date the activity took place cannot be in the future")
                .Must((command, commencedOn, token) => HaveOccurredOnOrAfterConsentWasGranted(command.ParticipantId, commencedOn))
                .WithMessage("The activity cannot take place before the participant gave consent");

            // ISW's can be claimed > 3 months ago. All other types of activity must be completed in last 3 months.
            When(c => c.Definition?.Classification.IsClaimableMoreThanThreeMonthsAgo is false, () =>
            {
                RuleFor(c => c.CommencedOn)
                    .GreaterThanOrEqualTo(DateTime.Today.AddMonths(-3))
                    .WithMessage("The activity must have taken place within the last three months");
            });

            RuleFor(c => c.AdditionalInformation)
                .NotEmpty()
                .WithMessage("Additional Information is required")
                .MaximumLength(ValidationConstants.NotesLength)
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Additional Information"));

            // Require document upload when:
            // The selected activity type requires further information
            When(c => c.Definition?.Classification.RequiresFurtherInformation is true, () =>
            {
                RuleFor(v => v.Document)
                        .NotNull()
                        .When(c => c.ActivityId is null)
                        .WithMessage("You must upload a Template");

                When(c => c.Document is not null, () =>
                {
                    RuleFor(v => v.Document)
                            .Must(file => NotExceedMaximumFileSize(file, Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes))
                            .WithMessage($"File size exceeds the maxmimum allowed size of {Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes} megabytes")
                            .MustAsync(BePdfFile)
                            .WithMessage("File is not a PDF");
                });

            });
        }

        private bool NotBeDifferentFromTheOriginal(Guid activityId, ActivityDefinition definition)
        {
            var activity = unitOfWork.DbContext.Activities.Single(a => a.Id == activityId);
            return activity.Definition == definition;
        }

        private bool BeInPendingStatus(Guid? activityId)
        {
            var activity = unitOfWork.DbContext.Activities.Single(a => a.Id == activityId);
            return activity.Status == ActivityStatus.PendingStatus;
        }

        private bool HaveAHubInduction(string participantId, LocationDto location)
        {
            return unitOfWork.DbContext.HubInductions.Any(induction => 
                induction.ParticipantId == participantId && induction.LocationId == location.Id);
        }

        private static bool NotExceedMaximumFileSize(IBrowserFile? file, double maxSizeMB)
        => file?.Size < ByteSize.FromMegabytes(maxSizeMB).Bytes;

        private async Task<bool> BePdfFile(IBrowserFile? file, CancellationToken cancellationToken)
        {
            if (file is null)
            {
                return false;
            }

            // Check file extension
            if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check MIME type
            if (file.ContentType != "application/pdf")
            {
                return false;
            }

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes).Bytes);

            // Check file signature (magic numbers)
            await using var stream = file.OpenReadStream(maxSizeBytes, cancellationToken);
            byte[] buffer = new byte[4];
            await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
            string header = System.Text.Encoding.ASCII.GetString(buffer);
            return header == "%PDF";
        }

        bool NotBeCompletedInTheFuture(DateTime? completed) => completed < DateTime.UtcNow;
        private bool HaveOccurredOnOrAfterConsentWasGranted(string participantId, DateTime? commencedOn)
        {
            if(commencedOn is null)
            {
                return false;
            }
            
            var participant = unitOfWork.DbContext
                                .Participants.Single(x => x.Id == participantId);

            return commencedOn >= participant.CalculateConsentDate();
        }
    }
}
