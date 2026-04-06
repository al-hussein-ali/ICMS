using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Geography
{
    public class GovernorateConfig : IEntityTypeConfiguration<Governorate>
    {
        public void Configure(EntityTypeBuilder<Governorate> builder)
        {
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
            
            builder.HasMany(g => g.Directorates)
                .WithOne(d => d.Governorate)
                .HasForeignKey(d => d.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
