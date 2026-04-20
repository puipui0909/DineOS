using DineOS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class Table
    {
        public Guid Id { get; private set; }
        public int TableNumber { get; private set; }
        public string? QrCode { get; private set; }
        public TableStatus Status { get; private set; }
        // Foreign key
        public Guid RestaurantId { get; set; }
        // Navigation
        public Restaurant Restaurant { get; set; } = null!;
        private readonly List<Order> _orders = new();
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

        private Table(Guid guid) { }

        public Table(int tableNumber, Guid restaurantId)
        {
            if (tableNumber <= 0)
                throw new ArgumentException("Invalid table number.");

            Id = Guid.NewGuid();
            TableNumber = tableNumber;
            RestaurantId = restaurantId;
            Status = TableStatus.Available;
            QrCode = $"https://dineos.app/order/{restaurantId}/{Id}";
        }

        public void MarkAsOccupied()
        {
            if (Status == TableStatus.Occupied)
                return;

            Status = TableStatus.Occupied;
        }

        public void MarkAsAvailable()
        {
            if (Status == TableStatus.Available)
                return;

            Status = TableStatus.Available;
        }
        public void MarkAsReserved()
        {
            if (Status == TableStatus.Occupied)
                throw new InvalidOperationException("Cannot reserve occupied table.");

            Status = TableStatus.Reserved;
        }

        public void MarkAsOutOfService()
        {
            Status = TableStatus.OutOfService;
        }

        public Order CreateOrder()
        {
            if (Status != TableStatus.Occupied)
                throw new InvalidOperationException("Table must be occupied");

            var order = Order.Create(Id);
            _orders.Add(order);

            return order;
        }
    }
}
