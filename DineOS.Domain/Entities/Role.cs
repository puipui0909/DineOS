using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        // Navigation
        private readonly List<User> _users = new();
        public ICollection<User> Users { get; set; } = new List<User>();

        private Role() { } // EF Core
        public Role(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name không được rỗng.");

            Id = Guid.NewGuid();
            Name = name;
        }


        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Role name không hợp lệ.");

            Name = newName;
        }
    }
}
