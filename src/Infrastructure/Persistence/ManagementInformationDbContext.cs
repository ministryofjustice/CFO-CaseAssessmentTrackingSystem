using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Infrastructure.Persistence.Configurations.ManagementInformation;

namespace Cfo.Cats.Infrastructure.Persistence;

public class ManagementInformationDbContext(DbContextOptions<ManagementInformationDbContext> options)
    : DbContext(options), IManagementInformationDbContext
{
    public DbSet<EnrolmentPayment> EnrolmentPayments => Set<EnrolmentPayment>();
    public DbSet<InductionPayment> InductionPayments => Set<InductionPayment>();
    public DbSet<ActivityPayment> ActivityPayments => Set<ActivityPayment>();

    public DbSet<DateDimension> DateDimensions => Set<DateDimension>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), x => x.Namespace!.StartsWith(typeof(EnrolmentPaymentEntityTypeConfiguration).Namespace!));
    }
}