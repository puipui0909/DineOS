using Xunit;
using FluentAssertions;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;

public class OrderTests
{
    [Fact]
    public void Create_Order_Should_Start_With_Open_Status()
    {
        var tableId = Guid.NewGuid();

        var order = Order.Create(tableId);

        order.Status.Should().Be(OrderStatus.Open);
        order.TotalAmount.Should().Be(0);
        order.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_Should_Add_New_Item_And_Update_Total()
    {
        var order = Order.Create(Guid.NewGuid());
        var menuItemId = Guid.NewGuid();

        order.AddItem(menuItemId, 2, 50);

        order.OrderItems.Count.Should().Be(1);
        order.TotalAmount.Should().Be(100);
    }

    [Fact]
    public void AddItem_Should_Merge_When_Item_Already_Exists()
    {
        var order = Order.Create(Guid.NewGuid());
        var menuItemId = Guid.NewGuid();

        order.AddItem(menuItemId, 2, 50);
        order.AddItem(menuItemId, 1, 50);

        order.OrderItems.Count.Should().Be(1);
        order.OrderItems.First().Quantity.Should().Be(3);
        order.TotalAmount.Should().Be(150);
    }

    [Fact]
    public void RemoveItem_Should_Remove_Item_And_Recalculate_Total()
    {
        var order = Order.Create(Guid.NewGuid());
        var menuItemId = Guid.NewGuid();
        order.AddItem(menuItemId, 2, 50);

        var itemId = order.OrderItems.First().Id;

        order.RemoveItem(itemId);

        order.OrderItems.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void RemoveItem_Should_Throw_When_Item_Not_Exists()
    {
        var order = Order.Create(Guid.NewGuid());

        Action act = () => order.RemoveItem(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RemoveItem_Should_Throw_When_Item_Already_Sent()
    {
        var order = Order.Create(Guid.NewGuid());
        var menuItemId = Guid.NewGuid();

        order.AddItem(menuItemId, 1, 100);

        // giả lập đã gửi bếp
        var item = order.OrderItems.First();
        item.MarkAsSent();

        Action act = () => order.RemoveItem(item.Id);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Close_Should_Throw_When_Order_Is_Empty()
    {
        var order = Order.Create(Guid.NewGuid());

        Action act = () => order.Close();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Close_Should_Set_Status_To_Closed()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 100);

        order.Close();

        order.Status.Should().Be(OrderStatus.Closed);
    }

    [Fact]
    public void AddItem_After_Close_Should_Create_New_Item()
    {
        var order = Order.Create(Guid.NewGuid());
        var menuItemId = Guid.NewGuid();

        order.AddItem(menuItemId, 1, 100);

        order.OrderItems.First().MarkAsSent();

        order.AddItem(menuItemId, 2, 100);

        order.OrderItems.Count.Should().Be(2);
    }

    [Fact]
    public void CreatePayment_Should_Throw_When_Order_Not_Closed()
    {
        var order = Order.Create(Guid.NewGuid());

        Action act = () => order.CompletePayment(PaymentMethod.Cash);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreatePayment_Should_Create_Payment_When_Order_Closed()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 2, 50);
        order.Close();

        order.CompletePayment(PaymentMethod.Cash);

        order.Payment.Should().NotBeNull();
        order.Payment!.Amount.Should().Be(order.TotalAmount);
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void CreatePayment_Should_Throw_When_Payment_Already_Exists()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 100);
        order.Close();

        order.CompletePayment(PaymentMethod.Cash);

        Action act = () => order.CompletePayment(PaymentMethod.Cash);

        act.Should().Throw<InvalidOperationException>();
    }


}