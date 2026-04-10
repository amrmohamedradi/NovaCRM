using FluentAssertions;
using NSubstitute;
using NovaCRM.Application.Features.Customers.Commands;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.UnitTests.Handlers;

public class DeleteCustomerCommandHandlerTests
{
    private readonly IRepository<Customer> _repo = Substitute.For<IRepository<Customer>>();
    private readonly DeleteCustomerCommandHandler _sut;

    public DeleteCustomerCommandHandlerTests()
    {
        _sut = new DeleteCustomerCommandHandler(_repo);
    }

    [Fact]
    public async Task Handle_deletes_customer_and_saves_changes()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer { Id = customerId, FullName = "Alice Johnson", Email = "alice@example.com" };
        _repo.GetByIdAsync(customerId).Returns(customer);

        var result = await _sut.Handle(new DeleteCustomerCommand(customerId), CancellationToken.None);

        result.Should().BeTrue();
        _repo.Received(1).Delete(customer);
        await _repo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_throws_when_customer_does_not_exist()
    {
        var customerId = Guid.NewGuid();
        _repo.GetByIdAsync(customerId).Returns((Customer?)null);

        var action = () => _sut.Handle(new DeleteCustomerCommand(customerId), CancellationToken.None);

        await action.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Customer {customerId} not found.");
    }
}
