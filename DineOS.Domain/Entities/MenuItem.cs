using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string? ImageUrl { get; private set; }

        public decimal Price { get; private set; }

        public bool IsAvailable { get; private set; } = true;

        // Foreign key
        public Guid CategoryId { get; private set; }

        // Navigation
        public Category Category { get; private set; } = null!;

        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

        private MenuItem() { } // EF Core

        public MenuItem(string name, decimal price, Guid categoryId, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên món không hợp lệ");

            if (price <= 0)
                throw new ArgumentException("Giá phải lớn hơn 0");

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            CategoryId = categoryId;
            ImageUrl = imageUrl;
            IsAvailable = true;
        }


        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Giá phải lớn hơn 0");

            Price = newPrice;
        }


        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Tên không hợp lệ");

            Name = newName;
        }


    }
}
