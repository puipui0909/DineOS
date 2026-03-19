using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .HasColumnName("id");

        builder.Property(u => u.Username)
               .HasColumnName("username")
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasColumnName("password_hash")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(u => u.RoleId)
               .HasColumnName("role_id")
               .IsRequired();

        builder.Property(u => u.RestaurantId)
       .HasColumnName("restaurant_id")
       .IsRequired();


        // USER – ROLE (*–1)
        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

        // USER – RESTAURANT (*–1)
        builder.HasOne(u => u.Restaurant)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RestaurantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
