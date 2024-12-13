using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Common.Interfaces;

public interface IManagementInformationDbContext
{
    DbSet<EnrolmentPayment> EnrolmentPayments { get; }
    DbSet<InductionPayment> InductionPayments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}