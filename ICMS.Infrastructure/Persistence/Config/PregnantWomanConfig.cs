using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PregnantWomanConfig : IEntityTypeConfiguration<PregnantWoman>
    {
        private string[] _bloodGroups = { "A", "B", "AB", "O" };
        private string[] _rhFactors = { "+", "-" };

        public void Configure(EntityTypeBuilder<PregnantWoman> builder)
        {
            builder.HasKey(pw => pw.Id);
            builder.Property(pw => pw.Id).HasColumnName("PregnantWomanId");

            builder.Property(pw => pw.CurrentAddress).HasMaxLength(250).IsUnicode(true).IsRequired();
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

            builder.HasIndex(vi => new { vi.PersonId, vi.UserId }, "IX_VaccinatedIndividuals_People_Users");

            builder.HasOne(pw => pw.Person)
                .WithOne(p => p.PregnantWoman)
                .HasForeignKey<PregnantWoman>(pw => pw.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pw => pw.User)
                .WithMany()
                .HasForeignKey(pw => pw.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pw => pw.PregnancyDetails)
                .WithOne(pd => pd.PregnantWoman)
                .HasForeignKey(pd => pd.PregnantWomanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(nameof(PregnantWoman.PregnancyDetails))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_pregnancyDetails");
        }
    }
}
