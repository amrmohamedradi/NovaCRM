using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Application.Interfaces;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;

namespace NovaCRM.Application.Features.Activities.Commands;
public record CreateActivityCommand(
    Guid CustomerId,
    Guid? DealId,
    ActivityType Type,
    string Description,
    DateTime DueDate) : IRequest<ActivityDto>;

public class CreateActivityCommandHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateActivityCommand, ActivityDto>
{
    public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken ct)
    {
        _ = await context.Customers.FindAsync(new object[] { request.CustomerId }, ct)
            ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var activity = new Activity
        {
            CustomerId  = request.CustomerId,
            DealId      = request.DealId,
            Type        = request.Type,
            Description = request.Description,
            DueDate     = request.DueDate,
            IsDone      = false
        };

        context.Activities.Add(activity);
        await context.SaveChangesAsync(ct);
        return mapper.Map<ActivityDto>(activity);
    }
}
