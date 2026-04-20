using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("tables");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.TableNumber).HasColumnName("table_number").IsRequired();

        builder.Property(t => t.RestaurantId).HasColumnName("restaurant_id").IsRequired();

        builder.Property(t => t.QrCode).HasColumnName("qr_code").HasMaxLength(255);

        builder.Property(t => t.Status).HasColumnName("status").IsRequired();

        // RESTAURANT – TABLE (1–*)
        builder.HasOne(t => t.Restaurant)        
               .WithMany(r => r.Tables)          
               .HasForeignKey(t => t.RestaurantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
