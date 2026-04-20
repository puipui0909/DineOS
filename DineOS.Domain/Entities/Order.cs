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
        public bool IsActive { get; set; }

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
            IsActive = true;
        }
        public static Order Create(Guid tableId) => new(tableId);

        public void AddItem(Guid menuItemId, int quantity, decimal unitPrice)
        {
            if (Status == OrderStatus.Closed || Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order cannot be modified.");

            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (unitPrice < 0)
                throw new ArgumentException("Giá không hợp lệ.");

            var existingItem = _orderItems
            .FirstOrDefault(x => x.MenuItemId == menuItemId && !x.IsSentToKitchen);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var item = new OrderItem(Id, menuItemId, quantity, unitPrice);
                _orderItems.Add(item);
            }
            RecalculateTotal();
        }

        public void RemoveItem(Guid orderItemId)
        {
            var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (item == null)
                throw new InvalidOperationException("Không tìm thấy món ăn trong đơn hàng.");
            if (item.IsSentToKitchen)
                throw new InvalidOperationException("Không thể xoá món đang được thực hiện.");
            if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order không thể được chỉnh sửa.");
            _orderItems.Remove(item);
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            TotalAmount = _orderItems.Sum(x => x.TotalPrice);
        }
        public void SendToKitchen()
        {
            if (Status == OrderStatus.Paid || Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is not editable.");

            var itemsToSend = _orderItems.Where(i => !i.IsSentToKitchen).ToList();

            if (!itemsToSend.Any())
                return;

            foreach (var item in itemsToSend)
            {
                item.MarkAsSent();
            }
            if (Status == OrderStatus.Open)
            {
                Status = OrderStatus.InProgress;
            }
        }

        public void Close()
        {
            if (Status != OrderStatus.Open && Status != OrderStatus.InProgress)
                throw new InvalidOperationException("Invalid order state.");

            var itemsToSend = _orderItems.Where(i => !i.IsSentToKitchen).ToList();

            if (!_orderItems.Any())
                throw new InvalidOperationException("Cannot close empty order.");
            if (_orderItems.Any(i => !i.IsSentToKitchen))
                throw new InvalidOperationException("All items must be sent before closing.");
            Status = OrderStatus.Closed;
        }

        public void Cancel()
        {
            if (Status != OrderStatus.Open)
                throw new InvalidOperationException("Only open orders can be cancelled.");

            Status = OrderStatus.Cancelled;
            IsActive = false;
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
            else if (method == PaymentMethod.Transfer)
            {
                payment.MarkAsPaid();
            }

            Payment = payment;
            Status = OrderStatus.Paid; 
            IsActive = false;

            Table?.MarkAsAvailable();
        }
    }
}
