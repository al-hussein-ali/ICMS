using ICMS.Domain.Entites.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Identity
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).HasMaxLength(250).IsRequired();
            builder.Property(n => n.Message).HasMaxLength(1000).IsRequired();
            builder.Property(n => n.TargetUrl).HasMaxLength(500).IsRequired(false);
            builder.Property(n => n.JobId).HasMaxLength(100).IsRequired(false);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(n => !n.User.Person.IsDeleted);
        }
    }
}
