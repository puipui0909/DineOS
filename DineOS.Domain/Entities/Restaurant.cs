using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class Restaurant
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string? Address { get; private set; }

        public DateTime CreatedAt { get; private set; }


        // Navigation
        private readonly List<Category> _categories = new();
        public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

        private readonly List<Table> _tables = new();
        public IReadOnlyCollection<Table> Tables => _tables.AsReadOnly();

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();


        private Restaurant() { } // EF Core
        public Restaurant(string name, string? address = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name không được rỗng.");

            Id = Guid.NewGuid();
            Name = name;
            Address = address;
            CreatedAt = DateTime.UtcNow;
        }


        public void UpdateInfo(string name, string? address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Restaurant name không hợp lệ.");

            Name = name;
            Address = address;
        }
    }
}

