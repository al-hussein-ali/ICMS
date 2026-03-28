using ICMS.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class VaccinatedIndividualConfig : IEntityTypeConfiguration<VaccinatedIndividual>
    {
        static string cardNumberPrefix = "AB";
        public void Configure(EntityTypeBuilder<VaccinatedIndividual> builder)
        {
            builder.HasKey(vi => vi.Id);
            builder.Property(vi => vi.Id).HasColumnName("VaccinatedIndividualId");

            builder.Property(vi => vi.CardNumber).HasMaxLength(60)
                .IsUnicode(false)
                .IsRequired()
                .HasDefaultValueSql($"'{cardNumberPrefix}' || LPAD(nextval('public.cardnumber_sequence')::text, 6, '0')")
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);



            builder.Property(vi => vi.Directorate).HasMaxLength(60).IsUnicode(true).IsRequired();
            builder.Property(vi => vi.Area).HasMaxLength(100).IsUnicode(true).IsRequired();
            builder.Property(vi => vi.Neighborhood).HasMaxLength(100).IsUnicode(true).IsRequired();

            builder.HasIndex(vi => vi.CardNumber).IsUnique(true);
            builder.HasIndex(vi => vi.PersonId).IsUnique(true);
            builder.HasIndex(vi => vi.UserId).IsUnique(true);

            builder.HasIndex(vi => new { vi.PersonId, vi.UserId }, "IX_VaccinatedIndividuals_People_Users");


            builder.HasOne(vi => vi.Person)
                .WithOne()
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
