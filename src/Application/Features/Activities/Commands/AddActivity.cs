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
    public class Command : IRequest<Result>
    {
        public Guid? ActivityId { get; set; }
        public required string ParticipantId { get; set; }
        public required Guid TaskId { get; set; }

        [Description("Location")]
        public LocationDto? Location { get; set; }

        [Description("Activity/ETE")]
        public ActivityDefinition? Definition { get; set; }

        [Description("Date activity took place")]
        public DateTime? CommencedOn { get; set; }

        [Description("Additional Information (optional)")]
        public string? AdditionalInformation { get; set; }

        public EmploymentDto EmploymentTemplate { get; set; } = new();
        public EducationTrainingDto EducationTrainingTemplate { get; set; } = new();
        public IswDto ISWTemplate { get; set; } = new();

        [Description("Upload Template")]
        public IBrowserFile? Document { get; set; }

        class Mapping : Profile
        {
            public Mapping()
            {
                //CreateMap<EmploymentActivity, EmploymentDto>();
                //CreateMap<EducationTrainingActivity, EducationTrainingDto>();
                //CreateMap<ISWActivity, IswDto>();

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
                    .ForMember(dest => dest.DomainEvents, opt => opt.Ignore())
                    .ForMember(dest => dest.Created, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
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
                activity.OwnerId = currentUserService.UserId;
                await unitOfWork.DbContext.Activities.AddAsync(activity, cancellationToken);
            }
            else
            {
                activity.ClearDomainEvents(); // Suppress "create" domain events
                var entity = await unitOfWork.DbContext.Activities.SingleAsync(a => a.Id == request.ActivityId, cancellationToken);
                mapper.Map(activity, entity); // Map activity with new values
                entity.TransitionTo(ActivityStatus.SubmittedToProviderStatus);
                unitOfWork.DbContext.Activities.Entry(entity).State = EntityState.Modified;
                entity.OwnerId = currentUserService.UserId;
                unitOfWork.DbContext.Activities.Update(entity);
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
                    .MustAsync(BeInPendingStatus);
            });

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9);

            RuleFor(c => c.Location)
                .NotNull()
                .WithMessage("You must choose a location");

            RuleFor(c => c.Location)
                .MustAsync(async (command, location, token) => await HaveAHubInduction(command.ParticipantId, location!, token))
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
                .WithMessage("The date the activity took place cannot be in the future");

            RuleFor(c => c.AdditionalInformation)
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
        
        async Task<bool> BeInPendingStatus(Guid? activityId, CancellationToken cancellationToken)
        {
            var activity = await unitOfWork.DbContext.Activities.SingleAsync(a => a.Id == activityId, cancellationToken);
            return activity.Status == ActivityStatus.PendingStatus;
        }

        async Task<bool> HaveAHubInduction(string participantId, LocationDto location, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.HubInductions.AnyAsync(induction => 
                induction.ParticipantId == participantId && induction.LocationId == location.Id, cancellationToken);
        }

        private static bool NotExceedMaximumFileSize(IBrowserFile? file, double maxSizeMB)
        => file?.Size < ByteSize.FromMegabytes(maxSizeMB).Bytes;

        private async Task<bool> BePdfFile(IBrowserFile? file, CancellationToken cancellationToken)
        {
            if (file is null)
                return false;

            // Check file extension
            if (!Path.GetExtension(file.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                return false;

            // Check MIME type
            if (file.ContentType != "application/pdf")
                return false;

            long maxSizeBytes = Convert.ToInt64(ByteSize.FromMegabytes(Infrastructure.Constants.Documents.ActivityTemplate.MaximumSizeInMegabytes).Bytes);

            // Check file signature (magic numbers)
            using (var stream = file.OpenReadStream(maxSizeBytes, cancellationToken))
            {
                byte[] buffer = new byte[4];
                await stream.ReadExactlyAsync(buffer.AsMemory(0, 4), cancellationToken);
                string header = System.Text.Encoding.ASCII.GetString(buffer);
                return header == "%PDF";
            }
        }

        bool NotBeCompletedInTheFuture(DateTime? completed) => completed < DateTime.UtcNow;
    }
}
