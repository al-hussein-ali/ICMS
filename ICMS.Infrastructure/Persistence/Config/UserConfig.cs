using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config;
public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("UserId");

        builder.Property(u => u.UserName).HasMaxLength(50).IsUnicode(true).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();

        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasIndex(u => u.PersonId);

        builder.HasOne(u => u.Person)
            .WithOne()
            .HasForeignKey<User>(u => u.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.FieldVisitUsers)
            .WithOne(fvu => fvu.FieldWorker)
            .HasForeignKey(fvu => fvu.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(nameof(User.UserRoles))?
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_userRoles");

        builder.Navigation(nameof(User.FieldVisitUsers))?
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_fieldVisitUsers");



    }
}
