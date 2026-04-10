using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Contacts.Queries;

public record GetContactsByCustomerIdQuery(Guid CustomerId) : IRequest<List<ContactDto>>;

public class GetContactsByCustomerIdQueryHandler(IRepository<Contact> repo, IMapper mapper)
    : IRequestHandler<GetContactsByCustomerIdQuery, List<ContactDto>>
{
    public async Task<List<ContactDto>> Handle(GetContactsByCustomerIdQuery request, CancellationToken ct)
    {

        var contacts = await repo.ExecuteAsync(
            repo.Query()
                .Where(c => c.CustomerId == request.CustomerId)
                .OrderBy(c => c.FullName),
            ct);

        return mapper.Map<List<ContactDto>>(contacts);
    }
}
