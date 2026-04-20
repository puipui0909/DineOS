using DineOS.Application.DTOs;
using DineOS.Application.Services;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using DineOS.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderServiceTests
{
    private readonly DineOSDbContext _context;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<DineOSDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DineOSDbContext(options);
        _service = new OrderService(_context);
    }

    [Fact]
    public async Task AddItemAsync_Should_Add_Item_To_Order()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem(
            "Pho",
            50000,
            categoryId
        );

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 2
        });

        var savedOrder = await _context.Orders
        .Include(o => o.OrderItems)
        .FirstAsync();

        savedOrder.OrderItems.Count.Should().Be(1);
        savedOrder.OrderItems.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task RemoveItemAsync_Should_Remove_Item()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem(
            "Pho",
            50000,
            categoryId
        );

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 2
        });

        var savedOrder = await _context.Orders.Include(o => o.OrderItems).FirstAsync();

        var orderItemId = savedOrder.OrderItems.First().Id;

        await _service.RemoveItemAsync(order.Id, orderItemId);

        savedOrder = await _context.Orders
        .Include(o => o.OrderItems)
        .FirstAsync();

        savedOrder.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public async Task CloseAsync_Should_Close_Order()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem(
            "Pho",
            50000,
            categoryId
        );

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        });

        await _service.CloseAsync(order.Id);

        var savedOrder = await _context.Orders.FirstAsync();

        savedOrder.Status.Should().Be(OrderStatus.Closed);
    }

    [Fact]
    public async Task SendToKitchen_Should_Mark_Items_As_Sent()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem("Pho", 50000, categoryId);

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add item
        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 2
        });

        // Send to kitchen
        await _service.SendToKitchenAsync(order.Id);

        var savedOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        savedOrder.OrderItems.All(i => i.IsSentToKitchen).Should().BeTrue();
    }

    [Fact]
    public async Task SendToKitchen_Should_Throw_When_No_New_Items()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem("Pho", 50000, categoryId);

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        });

        await _service.SendToKitchenAsync(order.Id);

        // Gửi lần 2 (không có item mới)
        Func<Task> act = async () =>
            await _service.SendToKitchenAsync(order.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddItem_After_Send_Should_Create_New_Unsents()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem("Pho", 50000, categoryId);

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Lần 1
        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        });

        await _service.SendToKitchenAsync(order.Id);

        // Lần 2 (gọi thêm)
        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 2
        });

        var savedOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        savedOrder.OrderItems.Count.Should().Be(2);

        savedOrder.OrderItems.Count(i => i.IsSentToKitchen).Should().Be(1);
        savedOrder.OrderItems.Count(i => !i.IsSentToKitchen).Should().Be(1);
    }

    [Fact]
    public async Task RemoveItem_Should_Throw_When_Item_Already_Sent()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem("Pho", 50000, categoryId);

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        });

        await _service.SendToKitchenAsync(order.Id);

        var savedOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        var itemId = savedOrder.OrderItems.First().Id;

        Func<Task> act = async () =>
            await _service.RemoveItemAsync(order.Id, itemId);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Close_Should_Not_Change_Sent_Status()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);
        var categoryId = Guid.NewGuid();

        var menuItem = new MenuItem("Pho", 50000, categoryId);

        _context.Tables.Add(table);
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.AddItemAsync(order.Id, new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        });

        await _service.SendToKitchenAsync(order.Id);

        await _service.CloseAsync(order.Id);

        var savedOrder = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        savedOrder.OrderItems.All(i => i.IsSentToKitchen).Should().BeTrue();
    }

    [Fact]
    public async Task CancelAsync_Should_Cancel_Order_And_Free_Table()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.CancelAsync(order.Id);

        var savedOrder = await _context.Orders
        .FirstAsync(o => o.Id == order.Id);
        var savedTable = await _context.Tables.FirstAsync();

        savedOrder.Status.Should().Be(OrderStatus.Cancelled);
        savedTable.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public async Task AddItemAsync_Should_Throw_When_Order_Not_Found()
    {
        Func<Task> act = async () =>
            await _service.AddItemAsync(Guid.NewGuid(), new AddOrderItemRequest
            {
                MenuItemId = Guid.NewGuid(),
                Quantity = 1
            });

        await act.Should().ThrowAsync<Exception>();
    }
}