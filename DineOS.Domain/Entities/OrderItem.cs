using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; private set; }

        // Foreign keys
        public Guid OrderId { get; private set; }
        public Guid MenuItemId { get; private set; }

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        // Navigation
        [System.Text.Json.Serialization.JsonIgnore]
        public Order? Order { get; private set; }
        public MenuItem? MenuItem { get; private set; }

        private OrderItem() { }
        internal OrderItem(Guid orderId, Guid menuItemId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Invalid quantity.");

            if (unitPrice < 0)
                throw new ArgumentException("Invalid price.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            MenuItemId = menuItemId;
            Quantity = quantity;
            UnitPrice = unitPrice;

            // Tận dụng hàm update để validate luôn số lượng
            UpdateQuantity(quantity);
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            Quantity = newQuantity;
        }
    }
}
    