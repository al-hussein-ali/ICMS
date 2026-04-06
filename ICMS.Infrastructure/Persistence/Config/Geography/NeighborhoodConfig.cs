using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Geography
{
    public class NeighborhoodConfig : IEntityTypeConfiguration<Neighborhood>
    {
        public void Configure(EntityTypeBuilder<Neighborhood> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Name).HasMaxLength(100).IsRequired();

            builder.HasMany(n => n.SubNeighborhoods)
                .WithOne(sn => sn.Neighborhood)
                .HasForeignKey(sn => sn.NeighborhoodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
