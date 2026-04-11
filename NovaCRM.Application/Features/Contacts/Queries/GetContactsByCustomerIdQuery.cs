using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;

namespace NovaCRM.Application.Features.Contacts.Queries;

public record GetContactsByCustomerIdQuery(Guid CustomerId) : IRequest<List<ContactDto>>;

public class GetContactsByCustomerIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetContactsByCustomerIdQuery, List<ContactDto>>
{
    public async Task<List<ContactDto>> Handle(GetContactsByCustomerIdQuery request, CancellationToken ct)
    {

        var contacts = await context.Contacts
            .AsNoTracking()
            .Where(c => c.CustomerId == request.CustomerId)
            .OrderBy(c => c.FullName)
            .ProjectTo<ContactDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return contacts;
    }
}
