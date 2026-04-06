using ICMS.Domain.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Identity
{
    public class VaccinatedIndividualConfig : IEntityTypeConfiguration<VaccinatedIndividual>
    {
        static string _cardNumberPrefix = "AB";

        public void Configure(EntityTypeBuilder<VaccinatedIndividual> builder)
        {
            builder.HasKey(vi => vi.Id);
            builder.Property(vi => vi.Id).HasColumnName("VaccinatedIndividualId");

            builder.Property(vi => vi.CardNumber).HasMaxLength(60)
                .IsUnicode(false)
                .IsRequired()
                .HasDefaultValueSql(
                    $"'{_cardNumberPrefix}' || LPAD(nextval('public.cardnumber_sequence')::text, 6, '0')")
                .ValueGeneratedOnAdd()
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);


            builder.Property(vi => vi.DirectorateId).IsRequired();
            builder.Property(vi => vi.NeighborhoodId).IsRequired();
            builder.Property(vi => vi.SubNeighborhoodId).IsRequired(false);

            builder.HasIndex(vi => vi.CardNumber).IsUnique();
            builder.HasIndex(vi => vi.PersonId).IsUnique();
            builder.HasIndex(vi => vi.UserId).IsUnique();

            builder.HasIndex(vi => new { vi.PersonId, vi.UserId }, "IX_VaccinatedIndividuals_People_Users");


            builder.HasOne(vi => vi.Person)
                .WithOne()
                .HasForeignKey<VaccinatedIndividual>(vi => vi.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vi => vi.User)
                .WithOne()
                .HasForeignKey<VaccinatedIndividual>(vi => vi.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Navigation(nameof(VaccinatedIndividual.ImmunizationRecords))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_immunizationRecords");

            builder.Navigation(nameof(VaccinatedIndividual.Schedules))?
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasField("_schedules");
        }
    }
}
