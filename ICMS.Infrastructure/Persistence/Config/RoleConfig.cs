using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("RoleId");

            builder.Property(r => r.RoleName).HasMaxLength(100).IsUnicode(true).IsRequired();

            builder.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(nameof(Role.UserRoles))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_userRoles");
        }
    }
}
