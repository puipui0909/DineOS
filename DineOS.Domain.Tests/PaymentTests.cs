using Xunit;
using FluentAssertions;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;

namespace DineOS.Domain.Tests;

public class PaymentTests
{
    [Fact]
    public void MarkAsPaid_Should_Set_Status_And_PaidAt()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 100);

        var payment = new Payment(
            order,                
            PaymentMethod.Cash
        );

        payment.MarkAsPaid();

        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsPaid_Should_Throw_When_Already_Paid()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 1, 100);

        var payment = new Payment(
            order,
            PaymentMethod.Cash
        );

        payment.MarkAsPaid();

        Action act = () => payment.MarkAsPaid();

        act.Should().Throw<InvalidOperationException>();
    }
}