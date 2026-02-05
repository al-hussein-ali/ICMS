using ICMS.Domain.Entites;
using ICMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Persistence.Config
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        private string[] _genders = { "ذكر", "انثى" };
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("PersonId");

            builder.Ignore(p => p.FullName);

            builder.Property(p => p.FirstName).HasMaxLength(20).IsUnicode(true).IsRequired();
            builder.Property(p => p.SecondName).HasMaxLength(20).IsUnicode(true).IsRequired();
            builder.Property(p => p.ThirdName).HasMaxLength(20).IsUnicode(true).IsRequired(false);
            builder.Property(p => p.LastName).HasMaxLength(20).IsUnicode(true).IsRequired();


            builder.HasIndex(p => new {p.PhoneNumber,p.FirstName,p.LastName,p.DateOfBirth}).IsUnique();


            builder.Property(p => p.DateOfBirth).IsRequired();

            builder.Property(p => p.Gender).HasConversion( v => v == Gender.Male ? _genders[0] : _genders[1], v => v.Equals(_genders[0]) ? Gender.Male : Gender.Female);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("TIMEZONE('utc', NOW())").ValueGeneratedOnAdd();

            builder.Property(p => p.IsDeleted).HasDefaultValue(false);





        }
    }
}
