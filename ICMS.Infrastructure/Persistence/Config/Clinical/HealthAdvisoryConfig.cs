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
using System.Reflection.Metadata.Ecma335;

namespace ICMS.Infrastructure.Persistence.Config.Clinical;

public class HealthAdvisoryConfig : IEntityTypeConfiguration<HealthAdvisory>
{
    private string[] _adviceTarget = { "????", "?????? ???????", "????????" };
    public void Configure(EntityTypeBuilder<HealthAdvisory> builder)
    {
        builder.HasKey(ha => ha.Id);

        builder.Property(ha => ha.Id).HasColumnName("HealthAdvisoryId");

        builder.Property(ha => ha.Title).HasMaxLength(250).IsUnicode(true).IsRequired();
        builder.Property(ha => ha.Content).HasMaxLength(2000).IsUnicode(true).IsRequired();

        builder.Property(ha => ha.Target).HasConversion(
            v => v != AdviceTarget.All ? v == AdviceTarget.PregnantWomen ? _adviceTarget[1] : _adviceTarget[2] : _adviceTarget[0],
            v => !v.Equals(_adviceTarget[0]) ? v.Equals(_adviceTarget[1]) ? AdviceTarget.PregnantWomen : AdviceTarget.VaccinatedIndividuals : AdviceTarget.All

        ).HasMaxLength(50).IsUnicode(true).IsRequired();
        

        builder.Property(ha => ha.CreationDate).HasDefaultValueSql("TIMEZONE('utc', NOW())").ValueGeneratedOnAdd();
        
        builder.Property(ha => ha.ScheduledDate).IsRequired();
        builder.Property(ha => ha.IsSent).HasDefaultValue(false);

        builder.HasIndex(ha => ha.UserId);

        builder.HasOne(ha => ha.User)
            .WithMany()
            .HasForeignKey(ha => ha.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(ha => !ha.User.Person.IsDeleted);
    }
}

