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
    public async Task OpenTableAsync_Should_Create_Order()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var orderId = await _service.OpenTableAsync(table.Id);

        var order = await _context.Orders.FirstOrDefaultAsync();

        order.Should().NotBeNull();
        order!.TableId.Should().Be(table.Id);
        order.Status.Should().Be(OrderStatus.Open);
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

        var orderId = await _service.OpenTableAsync(table.Id);

        await _service.AddItemAsync(orderId, menuItem.Id, 2);

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        order.OrderItems.Count.Should().Be(1);
        order.OrderItems.First().Quantity.Should().Be(2);
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

        var orderId = await _service.OpenTableAsync(table.Id);

        await _service.AddItemAsync(orderId, menuItem.Id, 1);

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        var orderItemId = order.OrderItems.First().Id;

        await _service.RemoveItemAsync(orderItemId);

        order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        order.OrderItems.Should().BeEmpty();
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

        var orderId = await _service.OpenTableAsync(table.Id);

        await _service.AddItemAsync(orderId, menuItem.Id, 1);

        await _service.CloseAsync(orderId);

        var order = await _context.Orders.FirstAsync();

        order.Status.Should().Be(OrderStatus.Closed);
    }

    [Fact]
    public async Task CancelAsync_Should_Cancel_Order_And_Free_Table()
    {
        var restaurantId = Guid.NewGuid();
        var table = new Table(1, restaurantId);

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var orderId = await _service.OpenTableAsync(table.Id);

        await _service.CancelAsync(orderId);

        var order = await _context.Orders.FirstAsync();
        var savedTable = await _context.Tables.FirstAsync();

        order.Status.Should().Be(OrderStatus.Cancelled);
        savedTable.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public async Task AddItemAsync_Should_Throw_When_Order_Not_Found()
    {
        Func<Task> act = async () =>
            await _service.AddItemAsync(Guid.NewGuid(), Guid.NewGuid(), 1);

        await act.Should().ThrowAsync<Exception>();
    }
}