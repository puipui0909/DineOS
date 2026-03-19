using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id).HasColumnName("id").HasColumnType("char(36)").ValueGeneratedNever();

        builder.Property(oi => oi.Quantity).HasColumnName("quantity").IsRequired();

        builder.Property(oi => oi.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(10,2)").IsRequired();

        builder.Property(oi => oi.OrderId).HasColumnName("order_id").HasColumnType("char(36)").IsRequired();

        builder.Property(oi => oi.MenuItemId).HasColumnName("menu_item_id").IsRequired();
    }
}
