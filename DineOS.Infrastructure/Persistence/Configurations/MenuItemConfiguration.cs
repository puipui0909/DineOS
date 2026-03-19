using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.Id).HasColumnName("id");

        builder.Property(mi => mi.Name).HasColumnName("name").HasMaxLength(255).IsRequired();

        builder.Property(mi => mi.ImageUrl).HasColumnName("image_url").HasMaxLength(500);

        builder.Property(mi => mi.Price).HasColumnName("price").HasColumnType("decimal(10,2)").IsRequired();

        builder.Property(mi => mi.IsAvailable).HasColumnName("is_available").IsRequired();

        builder.Property(mi => mi.CategoryId).HasColumnName("category_id").IsRequired();

        // Category – MenuItem (1–*)
        builder.HasOne(mi => mi.Category)
               .WithMany(c => c.MenuItems)
               .HasForeignKey(mi => mi.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
