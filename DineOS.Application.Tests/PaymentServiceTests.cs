using DineOS.Application.Common.Interfaces;
using DineOS.Application.Services;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using DineOS.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class PaymentServiceTests
{
    private readonly DineOSDbContext _context;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DineOSDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DineOSDbContext(options);
        _service = new PaymentService(_context);
    }

    [Fact]
    public async Task PayAsync_Should_Create_Payment()
    {
        var restaurantId = Guid.NewGuid();

        var table = new Table(1, restaurantId);

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        var order = Order.Create(table.Id);
        order.AddItem(Guid.NewGuid(), 1, 100);
        order.Close();

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _service.PayAsync(order.Id, PaymentMethod.Cash);

        var payment = await _context.Payments.FirstAsync();

        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.Amount.Should().Be(order.TotalAmount);
    }
}