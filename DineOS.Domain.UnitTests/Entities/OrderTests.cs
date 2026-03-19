using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using FluentAssertions; // Thư viện giúp câu lệnh Assert dễ đọc hơn

namespace DineOS.Domain.UnitTests.Entities;

public class OrderTests
{
    [Fact] // Đánh dấu đây là một hàm Test
    public void AddItem_WhenOrderIsOpen_ShouldIncreaseTotalAmount()
    {
        // 1. Arrange (Chuẩn bị dữ liệu)
        var tableId = Guid.NewGuid();
        var order = new Order(tableId);
        var menuItemId = Guid.NewGuid();

        // 2. Act (Thực hiện hành động cần test)
        order.AddItem(menuItemId, 2, 50000);

        // 3. Assert (Kiểm tra kết quả)
        // Tổng tiền phải là 100,000 (2 món x 50,000)
        order.TotalAmount.Should().Be(100000);
        order.OrderItems.Should().HaveCount(1);
    }
}