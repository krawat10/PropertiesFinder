using IntegrationApi.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseConnection.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(log => log.Id);
            builder.Property(log => log.Id).ValueGeneratedOnAdd();

            builder.Property(log => log.Time).ValueGeneratedOnAdd();

            builder.Property(log => log.HeaderValue).IsRequired();
        }
    }
}