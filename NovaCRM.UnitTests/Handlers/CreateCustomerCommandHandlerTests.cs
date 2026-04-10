using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Customers.Commands;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.UnitTests.Handlers;

public class CreateCustomerCommandHandlerTests
{
    private readonly IRepository<Customer> _repo = Substitute.For<IRepository<Customer>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly CreateCustomerCommandHandler _sut;

    public CreateCustomerCommandHandlerTests()
    {
        _sut = new CreateCustomerCommandHandler(_repo, _mapper);
    }

    [Fact]
    public async Task Handle_persists_customer_with_correct_properties()
    {
        var cmd = new CreateCustomerCommand("Alice", "alice@example.com", "555-1234", "Acme", CustomerStatus.Active);
        _mapper.Map<CustomerDto>(Arg.Any<Customer>())
            .Returns(new CustomerDto { FullName = "Alice", Email = "alice@example.com" });

        await _sut.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).AddAsync(Arg.Is<Customer>(c =>
            c.FullName == "Alice" &&
            c.Email    == "alice@example.com" &&
            c.Status   == CustomerStatus.Active));
    }

    [Fact]
    public async Task Handle_calls_save_changes()
    {
        var cmd = new CreateCustomerCommand("Bob", "bob@example.com", null, null, CustomerStatus.Lead);
        _mapper.Map<CustomerDto>(Arg.Any<Customer>())
            .Returns(new CustomerDto { FullName = "Bob", Email = "bob@example.com" });

        await _sut.Handle(cmd, CancellationToken.None);

        await _repo.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_returns_mapped_dto()
    {
        var expected = new CustomerDto { FullName = "Carol", Email = "carol@example.com" };
        var cmd = new CreateCustomerCommand("Carol", "carol@example.com", null, null, CustomerStatus.Lead);
        _mapper.Map<CustomerDto>(Arg.Any<Customer>()).Returns(expected);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.Should().BeSameAs(expected);
    }
}
