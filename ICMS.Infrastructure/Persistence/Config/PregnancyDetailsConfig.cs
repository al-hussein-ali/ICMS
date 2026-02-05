using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PregnancyDetailsConfig : IEntityTypeConfiguration<PregnancyDetails>
    {
        private string[] _pregnancyTypes = { "طبيعي", "مضاعفات" };
        private string[] _birthNatures = { "طبيعي", "ولادة قيصرية", "بالشفط", "بالملقاط" };
        private string[] _birthLocationTypes = { "المنزل", "الوحدة", "المركز", "المستشفى" };

        public void Configure(EntityTypeBuilder<PregnancyDetails> builder)
        {
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.Id).HasColumnName("PregnancyDetailsId");

            builder.Property(pd => pd.VisitsCount).HasDefaultValue(0);

            builder.Property(pd => pd.LastMenstrualPeriodDate).IsRequired();
            builder.Property(pd => pd.ExpectedDeliveryDate).IsRequired();
            builder.Property(pd => pd.DeliveryDate).IsRequired(false);

            builder.Property(pd => pd.PregnancyType).HasConversion(
                v => v == PregnancyType.Normal ? _pregnancyTypes[0] : _pregnancyTypes[1],
                v => v == _pregnancyTypes[0] ? PregnancyType.Normal : PregnancyType.Complicated
            ).IsRequired();


            builder.Property(pd => pd.BirthNature).HasConversion(
                v => v == BirthNature.Natural ? _birthNatures[0]
                      : v == BirthNature.CesareanSection ? _birthNatures[1]
                      : v == BirthNature.VacuumExtraction ? _birthNatures[2]
                      : _birthNatures[3],
                v => v == _birthNatures[0] ? BirthNature.Natural
                      : v == _birthNatures[1] ? BirthNature.CesareanSection
                      : v == _birthNatures[2] ? BirthNature.VacuumExtraction
                      : BirthNature.ForcepsDelivery
            ).IsRequired();


            builder.Property(pd => pd.BirthLocationType).HasConversion(
                v => v == BirthLocationType.AtHome ? _birthLocationTypes[0]
                      : v == BirthLocationType.AtTheUnit ? _birthLocationTypes[1]
                      : v == BirthLocationType.AtTheCenter ? _birthLocationTypes[2]
                      : _birthLocationTypes[3],
                v => v == _birthLocationTypes[0] ? BirthLocationType.AtHome
                      : v == _birthLocationTypes[1] ? BirthLocationType.AtTheUnit
                      : v == _birthLocationTypes[2] ? BirthLocationType.AtTheCenter
                      : BirthLocationType.AtTheHospital
            ).IsRequired();

            builder.Property(pd => pd.BirthLocationDetails).HasMaxLength(250).IsUnicode(true).IsRequired(false);
            builder.Property(pd => pd.BirthNatureReason).HasMaxLength(250).IsUnicode(true).IsRequired();
            builder.Property(pd => pd.PregnancyComplications).HasMaxLength(500).IsUnicode(true).IsRequired();
            builder.Property(pd => pd.Interferences).HasMaxLength(500).IsUnicode(true).IsRequired();

            builder.Property(pd => pd.NewbornCount).HasDefaultValue(0);
            builder.Property(pd => pd.IsPregnancyDone).HasDefaultValue(false);
            builder.Property(pd => pd.ComplicationsDuringChildbirth).HasMaxLength(500).IsUnicode(true).IsRequired(false);
            builder.Property(pd => pd.PostpartumComplications).HasMaxLength(500).IsUnicode(true).IsRequired(false);



            builder.HasIndex(pd => pd.PreviousPostartumComplicationsId);
            builder.HasIndex(pd => pd.PreviousPregnancyComplicationsId);
            builder.HasIndex(pd => pd.PreviousPregnancyDeliveryComplicationsId);

            builder.HasOne(pd => pd.PregnantWoman)
                .WithMany(pw => pw.PregnancyDetails)
                .HasForeignKey(pd => pd.PregnantWomanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pd => pd.PreviousPregnancyComplications)
                .WithOne(ppc => ppc.PregnancyDetails)
                .HasForeignKey<PreviousPregnancyComplications>(ppc => ppc.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pd => pd.PreviousPostartumComplications)
                .WithOne(pp => pp.PregnancyDetails)
                .HasForeignKey<PreviousPostartumComplications>(pp => pp.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pd => pd.PreviousPregnancyDelivaryComplications)
                .WithOne(ppd => ppd.PregnancyDetails)
                .HasForeignKey<PreviousPregnancyDeliveryComplications>(ppd => ppd.PregnancyDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pd => pd.VisitDetails)
                .WithOne(vd => vd.PregnancyDetails)
                .HasForeignKey(vd => vd.PregnancyDetailsId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
