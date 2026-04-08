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
    public class NewbornConfig : IEntityTypeConfiguration<Newborn>
    {
        private string[] _newbornStatuses = { "??", "???" };
        private string[] _genders = { "???", "????" };

        public void Configure(EntityTypeBuilder<Newborn> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id).HasColumnName("NewbornId");


            builder.Property(n => n.NewbornStatus).HasConversion(
                v => v == NewbornStatus.Alive ? _newbornStatuses[0] : _newbornStatuses[1],
                v => v == _newbornStatuses[0] ? NewbornStatus.Alive : NewbornStatus.Dead
            ).IsRequired();

            builder.Property(n => n.NewbornWeightInGrams).HasPrecision(8,2).IsRequired();

            builder.Property(n => n.NewbornGender).HasConversion(
                v => v == Gender.Male ? _genders[0] : _genders[1],
                v => v == _genders[0] ? Gender.Male : Gender.Female
            ).IsRequired();

            builder.HasIndex(nb => nb.PregnancyDetailsId);

            builder.HasOne(n => n.PregnancyDetails)
                .WithMany()
                .HasForeignKey(n => n.PregnancyDetailsId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
