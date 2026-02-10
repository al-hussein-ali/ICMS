using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class VaccinatedIndividualConfig : IEntityTypeConfiguration<VaccinatedIndividual>
    {
        public void Configure(EntityTypeBuilder<VaccinatedIndividual> builder)
        {
            builder.HasKey(vi => vi.Id);
            builder.Property(vi => vi.Id).HasColumnName("VaccinatedIndividualId");

            builder.Property(vi => vi.CardNumber).HasMaxLength(100).IsUnicode(false).IsRequired();
            builder.Property(vi => vi.Directorate).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(vi => vi.Area).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(vi => vi.Neighborhood).HasMaxLength(100).IsUnicode(true).IsRequired();

            builder.HasIndex(vi => vi.CardNumber).IsUnique(true);
            builder.HasIndex(vi => vi.PersonId).IsUnique(true);
            builder.HasIndex(vi => vi.UserId).IsUnique(true);

            builder.HasIndex(vi => new { vi.PersonId ,vi.UserId}, "IX_VaccinatedIndividuals_People_Users");

            
            builder.HasOne(vi => vi.Person)
                .WithOne(p => p.VaccinatedIndividual)
                .HasForeignKey<VaccinatedIndividual>(vi => vi.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vi => vi.User)
                .WithMany()
                .HasForeignKey(vi => vi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(nameof(VaccinatedIndividual.ImmunizationRecords))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_immunizationRecords");
        }
    }
}
