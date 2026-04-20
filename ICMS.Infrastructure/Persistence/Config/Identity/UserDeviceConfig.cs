using ICMS.Domain.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Identity
{
    public class UserDeviceConfig : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.HasKey(ud => ud.Id);
            builder.Property(ud => ud.Id).HasColumnName("UserDeviceId");

            builder.Property(ud => ud.FcmToken).HasMaxLength(500).IsRequired();
            builder.Property(ud => ud.LastActiveAt).IsRequired();

            builder.HasIndex(ud => ud.FcmToken).IsUnique();

            builder.HasOne(ud => ud.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
