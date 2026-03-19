using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurants");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        builder.Property(r => r.Address).HasColumnName("address").HasMaxLength(255);

        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();

        // CATEGORY-RESTAURANT (*-1)
        builder.HasMany(r => r.Categories)
               .WithOne(c => c.Restaurant)
               .HasForeignKey(c => c.RestaurantId)
               .OnDelete(DeleteBehavior.Cascade);

        // USER-RESTAURANT (*-1)
        builder.HasMany(r => r.Users)
               .WithOne(u => u.Restaurant)
               .HasForeignKey(u => u.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);

        // TABLE-RESTAURANT (*-1)
        builder.HasMany(r => r.Tables)
               .WithOne(t => t.Restaurant)
               .HasForeignKey(t => t.RestaurantId)
               .OnDelete(DeleteBehavior.Cascade);

        // TABLE-RESTAURANT (*-1)
        builder.HasMany(r => r.Tables)
                .WithOne(t => t.Restaurant)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

    }
}
