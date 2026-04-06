using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Geography
{
    public class DirectorateConfig : IEntityTypeConfiguration<Directorate>
    {
        public void Configure(EntityTypeBuilder<Directorate> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasMaxLength(100).IsRequired();

            builder.HasMany(d => d.Neighborhoods)
                .WithOne(n => n.Directorate)
                .HasForeignKey(n => n.DirectorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
