using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Data.Models;

namespace Test.Data.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Salary)
                .HasPrecision(18, 4);

            builder.Property(c => c.Name)
                .HasMaxLength(100);

            builder.Property(c => c.Phone)
                .HasMaxLength(15);

            builder.ToTable("Contacts");
        }
    }
}
