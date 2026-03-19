using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id).HasColumnType("char(36)").HasColumnName("id").ValueGeneratedNever();

        builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(o => o.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(12,2)").IsRequired();

        builder.Property(o => o.Status).HasColumnName("status").IsRequired();

        builder.Property(o => o.TableId).HasColumnName("table_id").IsRequired();

        // TABLE-ORDER (1-*)
        builder.HasOne(o => o.Table)
               .WithMany(t => t.Orders)
               .HasForeignKey(o => o.TableId)
               .OnDelete(DeleteBehavior.Restrict);
        // ORDER-ORDERITEM (1-*)
        builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        // ORDER-PAYMENT (1-1)
        builder.HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.OrderItems)
               .HasField("_orderItems")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
