using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace DatabaseConnection.Configurations
{
    public class EntryConfiguration: IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> builder)
        {
            builder.HasKey(entry => entry.Id);
            builder.Property(entry => entry.Id).ValueGeneratedOnAdd();

            builder.OwnsOne(x => x.PropertyFeatures);
            builder.OwnsOne(x => x.PropertyDetails);
            builder.OwnsOne(x => x.OfferDetails);
            builder.OwnsOne(x => x.PropertyAddress);
            builder.OwnsOne(x => x.PropertyPrice);
        }
    }
}