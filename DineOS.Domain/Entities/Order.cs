using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DineOS.Domain.Enums;

namespace DineOS.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid TableId { get; private set; }  // Foreign key
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        private readonly List<OrderItem> _orderItems = new ();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        // Navigation
        public Table? Table { get; private set; }
        public Payment? Payment { get; private set; }

        private Order() { }
        private Order(Guid tableId)
        {
            if (tableId == Guid.Empty)
                throw new ArgumentException("TableId không hợp lệ.");
            Id = Guid.NewGuid();
            TableId = tableId;
            Status = OrderStatus.Open;
            TotalAmount = 0;
            CreatedAt = DateTime.UtcNow;
        }
        public static Order Create(Guid tableId) => new(tableId);

        public void AddItem(Guid menuItemId, int quantity, decimal unitPrice)
        {
            EnsureOpen();
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (unitPrice < 0)
                throw new ArgumentException("Giá không hợp lệ.");

            var existingItem = _orderItems.FirstOrDefault(x => x.MenuItemId == menuItemId);

            if (existingItem != null)
            {
                // Cập nhật số lượng nếu món ăn đã tồn tại
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                // Tạo OrderItem mới với order_id, menu_item_id, quantity, unit_price
                var item = new OrderItem(Id, menuItemId, quantity, unitPrice);
                _orderItems.Add(item);
            }

            RecalculateTotal();
        }

        public void RemoveItem(Guid menuItemId)
        {
            EnsureOpen();

            var item = _orderItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (item == null)
                throw new InvalidOperationException("Không tìm thấy món ăn trong đơn hàng.");

            _orderItems.Remove(item);
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            TotalAmount = _orderItems.Sum(x => x.TotalPrice);
        }

        public void Close()
        {
            if (Status != OrderStatus.Open)
                throw new InvalidOperationException("Only open orders can be closed.");

            if (!_orderItems.Any())
                throw new InvalidOperationException("Cannot close empty order.");

            Status = OrderStatus.Closed;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Open)
                throw new InvalidOperationException("Only open orders can be cancelled.");

            Status = OrderStatus.Cancelled;
            Table?.MarkAsAvailable();
        }

        public void CompletePayment(PaymentMethod method)
        {
            if (Status != OrderStatus.Closed)
                throw new InvalidOperationException("Only closed orders can be paid.");

            if (Payment != null)
                throw new InvalidOperationException("Order already has a payment.");

            var payment = new Payment(this, method);
            if (method == PaymentMethod.Cash)
            {
                payment.MarkAsPaid();
            }

            Payment = payment;
            Status = OrderStatus.Paid;

            Table?.MarkAsAvailable();
        }

        public void RemoveItemById(Guid orderItemId)
        {
            EnsureOpen();

            var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (item == null)
                throw new InvalidOperationException("Item not found.");

            _orderItems.Remove(item);
            RecalculateTotal();
        }

        private void EnsureOpen()
        {
            if (Status != OrderStatus.Open)
                throw new InvalidOperationException("Order is not editable.");
        }
    }
}
