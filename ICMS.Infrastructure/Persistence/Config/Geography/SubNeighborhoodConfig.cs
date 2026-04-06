using ICMS.Domain.Entites.Geography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICMS.Infrastructure.Persistence.Config.Geography
{
    public class SubNeighborhoodConfig : IEntityTypeConfiguration<SubNeighborhood>
    {
        public void Configure(EntityTypeBuilder<SubNeighborhood> builder)
        {
            builder.HasKey(sn => sn.Id);
            builder.Property(sn => sn.Name).HasMaxLength(100).IsRequired();
        }
    }
}
