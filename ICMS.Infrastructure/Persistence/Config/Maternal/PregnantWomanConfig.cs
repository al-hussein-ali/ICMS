using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Maternal
{
    public class PregnantWomanConfig : IEntityTypeConfiguration<PregnantWoman>
    {
        private string[] _bloodGroups = { "A", "B", "AB", "O" };
        private string[] _rhFactors = { "+", "-" };

        public void Configure(EntityTypeBuilder<PregnantWoman> builder)
        {
            builder.HasKey(pw => pw.Id);
            builder.Property(pw => pw.Id).HasColumnName("PregnantWomanId");

            builder.Property(pw => pw.AgeRange).HasMaxLength(50).IsUnicode(true).IsRequired();
            builder.Property(pw => pw.PregnancyCount).IsRequired();

            builder.Property(pw => pw.BloodGroup).HasConversion(
                v => v == BloodGroup.A ? _bloodGroups[0]
                      : v == BloodGroup.B ? _bloodGroups[1]
                      : v == BloodGroup.AB ? _bloodGroups[2]
                      : _bloodGroups[3],
                v => v == _bloodGroups[0] ? BloodGroup.A
                      : v == _bloodGroups[1] ? BloodGroup.B
                      : v == _bloodGroups[2] ? BloodGroup.AB
                      : BloodGroup.O
            ).IsRequired();

            builder.Property(pw => pw.RhFactor).HasConversion(
                v => v == RhFactor.Positive ? _rhFactors[0] : _rhFactors[1],
                v => v == _rhFactors[0] ? RhFactor.Positive : RhFactor.Negative
            ).IsRequired();

            builder.HasIndex(pw => new { pw.PersonId, pw.UserId }, "IX_PregnantWomen_People_Users");
            builder.HasIndex(pw => pw.UserId).IsUnique();

            builder.HasOne(pw => pw.User)
                .WithOne()
                .HasForeignKey<PregnantWoman>(pw => pw.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pw => pw.Person)
                .WithOne()
                .HasForeignKey<PregnantWoman>(pw => pw.PersonId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Metadata.FindNavigation(nameof(PregnantWoman.PregnancyDetails))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasQueryFilter(pw => !pw.Person.IsDeleted);
        }
    }
}
