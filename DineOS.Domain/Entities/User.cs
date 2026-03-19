using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }

        public string Username { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        // Foreign key
        public Guid RoleId { get; private set; }

        public Guid RestaurantId { get; private set; }

        // Navigation
        public Role Role { get; private set; } = null!;

        public Restaurant Restaurant { get; private set; } = null!;

        private User() { } // Required for EF Core


        public User(
            string username,
            string passwordHash,
            Guid roleId,
            Guid restaurantId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username không được rỗng.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash không được rỗng.");

            Id = Guid.NewGuid();
            Username = username;
            PasswordHash = passwordHash;
            RoleId = roleId;
            RestaurantId = restaurantId;
        }


        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("PasswordHash không hợp lệ.");

            PasswordHash = newPasswordHash;
        }


        public void ChangeRole(Guid newRoleId)
        {
            if (newRoleId == Guid.Empty)
                throw new ArgumentException("RoleId không hợp lệ.");

            RoleId = newRoleId;
        }
    }
}

