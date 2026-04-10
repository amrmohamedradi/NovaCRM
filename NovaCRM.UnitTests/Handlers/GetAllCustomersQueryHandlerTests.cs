using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Features.Customers.Queries;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.UnitTests.Handlers;

public class GetAllCustomersQueryHandlerTests
{
    private readonly IRepository<Customer> _repo = Substitute.For<IRepository<Customer>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly GetAllCustomersQueryHandler _sut;

    public GetAllCustomersQueryHandlerTests()
    {
        _sut = new GetAllCustomersQueryHandler(_repo, _mapper);
    }

    private List<Customer> SampleCustomers() =>
    [
        new() { FullName = "Alice", Email = "alice@example.com" },
        new() { FullName = "Bob",   Email = "bob@example.com"   }
    ];

    [Fact]
    public async Task Handle_returns_correct_total_count()
    {
        var customers = SampleCustomers();
        _repo.Query().Returns(customers.AsQueryable());
        _repo.CountAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(2);
        _repo.ExecuteAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(customers);
        _mapper.Map<List<CustomerDto>>(Arg.Any<IEnumerable<Customer>>())
            .Returns(customers.Select(c => new CustomerDto { FullName = c.FullName }).ToList());

        var result = await _sut.Handle(new GetAllCustomersQuery(1, 10, null), CancellationToken.None);

        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_returns_correct_page_metadata()
    {
        var customers = SampleCustomers();
        _repo.Query().Returns(customers.AsQueryable());
        _repo.CountAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(2);
        _repo.ExecuteAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(customers);
        _mapper.Map<List<CustomerDto>>(Arg.Any<IEnumerable<Customer>>())
            .Returns(customers.Select(c => new CustomerDto { FullName = c.FullName }).ToList());

        var result = await _sut.Handle(new GetAllCustomersQuery(2, 5, null), CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_returns_items_from_mapper()
    {
        var customers = SampleCustomers();
        var dtos = customers.Select(c => new CustomerDto { FullName = c.FullName }).ToList();
        _repo.Query().Returns(customers.AsQueryable());
        _repo.CountAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(2);
        _repo.ExecuteAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(customers);
        _mapper.Map<List<CustomerDto>>(Arg.Any<IEnumerable<Customer>>()).Returns(dtos);

        var result = await _sut.Handle(new GetAllCustomersQuery(1, 10, null), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Select(i => i.FullName).Should().BeEquivalentTo(["Alice", "Bob"]);
    }

    [Fact]
    public async Task Handle_with_search_still_returns_result()
    {
        var customers = SampleCustomers().Take(1).ToList();
        _repo.Query().Returns(customers.AsQueryable());
        _repo.CountAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(1);
        _repo.ExecuteAsync(Arg.Any<IQueryable<Customer>>(), Arg.Any<CancellationToken>()).Returns(customers);
        _mapper.Map<List<CustomerDto>>(Arg.Any<IEnumerable<Customer>>())
            .Returns([new CustomerDto { FullName = "Alice" }]);

        var result = await _sut.Handle(new GetAllCustomersQuery(1, 10, "alice"), CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }
}
