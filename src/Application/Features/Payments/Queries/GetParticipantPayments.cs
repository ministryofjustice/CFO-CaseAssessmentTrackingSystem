using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetParticipantPayments
{
    [RequestAuthorize(Policy = SecurityPolicies.UserHasAdditionalRoles)]
    public class Query : IAuditableRequest<Result<ParticipantPaymentsDto>>
    {
        public required string ParticipantId { get; init; }
        public bool IncludeNonPayable { get; init; }

        public string Identifier() => ParticipantId;
    }

    public class Handler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IContractService contractService) : IQueryHandler<Query, Result<ParticipantPaymentsDto>>
    {
        public async Task<Result<ParticipantPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(currentUserService.TenantId))
            {
                return Result<ParticipantPaymentsDto>.Success(new ParticipantPaymentsDto());
            }

            // Only payments made against contracts the current user can see are ever returned.
            var contractIds = contractService
                .GetVisibleContracts(currentUserService.TenantId)
                .Select(c => c.Id)
                .ToList();

            var db = unitOfWork.DbContext;
            var participantId = request.ParticipantId;
            var includeNonPayable = request.IncludeNonPayable;
            var items = new List<ParticipantPaymentDto>();

            items.AddRange(await (
                from p in db.EnrolmentPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                join u in db.Users on p.SupportWorker equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Enrolment",
                    Date = p.Approved,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    SupportWorkerName = u.DisplayName ?? "",
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.InductionPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                join u in db.Users on p.SupportWorker equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Induction",
                    Description = p.LocationType,
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.Induction,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    SupportWorkerName = u.DisplayName ?? "",
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.SupportAndReferralPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                join u in db.Users on p.SupportWorker equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Support & Referral",
                    Description = p.SupportType,
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.Approved,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    SupportWorkerName = u.DisplayName ?? "",
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.ActivityPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Activity",
                    Description = p.ActivityType + " - " + p.ActivityCategory,
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.CommencedDate,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.EducationPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Education & Training",
                    Description = p.CourseTitle + " (" + p.CourseLevel + ")",
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.CommencedDate,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.EmploymentPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Employment",
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.CommencedDate,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            items.AddRange(await (
                from p in db.ReassessmentPayments
                join c in db.Contracts on p.ContractId equals c.Id
                join l in db.Locations on p.LocationId equals l.Id
                join u in db.Users on p.SupportWorker equals u.Id into userJoin
                from u in userJoin.DefaultIfEmpty()
                where p.ParticipantId == participantId
                      && contractIds.Contains(p.ContractId)
                      && (includeNonPayable || p.EligibleForPayment)
                select new ParticipantPaymentDto
                {
                    PaymentType = "Reassessment",
                    PaymentPeriod = p.PaymentPeriod,
                    Date = p.AssessmentCompleted,
                    Contract = c.Description,
                    Location = l.Name,
                    LocationType = p.LocationType,
                    SupportWorkerName = u.DisplayName ?? "",
                    EligibleForPayment = p.EligibleForPayment,
                    IneligibilityReason = p.IneligibilityReason
                }).AsNoTracking().ToListAsync(cancellationToken));

            return Result<ParticipantPaymentsDto>.Success(new ParticipantPaymentsDto
            {
                Items = items
                    .OrderByDescending(i => i.Date)
                    .ThenBy(i => i.PaymentType)
                    .ToArray()
            });
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}
