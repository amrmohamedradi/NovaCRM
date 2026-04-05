using AutoMapper;
using MediatR;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Enums;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Activities.Commands;
public record CreateActivityCommand(
    Guid CustomerId,
    Guid? DealId,
    ActivityType Type,
    string Description,
    DateTime DueDate) : IRequest<ActivityDto>;

public class CreateActivityCommandHandler(
    IRepository<Activity> repo,
    IRepository<Customer> customerRepo,
    IMapper mapper)
    : IRequestHandler<CreateActivityCommand, ActivityDto>
{
    public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken ct)
    {
        _ = await customerRepo.GetByIdAsync(request.CustomerId)
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

        await repo.AddAsync(activity);
        await repo.SaveChangesAsync();
        return mapper.Map<ActivityDto>(activity);
    }
}



