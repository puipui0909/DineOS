using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        // Foreign key
        public Guid RestaurantId { get; private set; }

        // Navigation
        public Restaurant Restaurant { get; private set; } = null!;

        private readonly List<MenuItem> _menuItems = new();
        public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

        private Category() { } // EF Core

        public Category(string name, Guid restaurantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name không được rỗng.");

            Id = Guid.NewGuid();
            Name = name;
            RestaurantId = restaurantId;
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name không hợp lệ.");

            Name = name;
        }
    }
}
