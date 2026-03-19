using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)").IsRequired();

        builder.Property(p => p.Method).HasColumnName("method").HasConversion<string>().IsRequired();

        builder.Property(p => p.OrderId).HasColumnName("order_id").IsRequired();

        builder.Property(p => p.Status).HasColumnName("status").IsRequired();

        builder.Property(p => p.PaidAt).HasColumnName("paid_at");

        // Order – Payment (1–0..1)
        builder.HasOne(p => p.Order)
               .WithOne(o => o.Payment)
               .HasForeignKey<Payment>(p => p.OrderId)
               .OnDelete(DeleteBehavior.Restrict);

        // đảm bảo 1 payment cho 1 order
        builder.HasIndex(p => p.OrderId).IsUnique();
    }
}
