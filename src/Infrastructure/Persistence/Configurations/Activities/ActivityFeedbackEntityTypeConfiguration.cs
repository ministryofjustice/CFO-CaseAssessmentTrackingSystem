internal class ActivityFeedbackEntityTypeConfiguration
    : IEntityTypeConfiguration<ActivityFeedback>
{
    public void Configure(EntityTypeBuilder<ActivityFeedback> builder)
    {
        builder.ToTable(
            DatabaseConstants.Tables.ActivityFeedback,
            DatabaseConstants.Schemas.Activities);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(ValidationConstants.NotesLength);

        builder.Property(x => x.RecipientUserId)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(DatabaseConstants.FieldLengths.GuidId);

        builder.HasOne(x => x.Activity)
            .WithMany() // or Activity.Feedback if you add a nav
            .HasForeignKey(x => x.ActivityId);

        builder.HasOne(x => x.RecipientUser)
            .WithMany()
            .HasForeignKey(x => x.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsRead)
            .IsRequired();

        builder.Property(x => x.Created)
            .IsRequired();
    }
}