using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Activities.Commands;
public record MarkActivityAsDoneCommand(Guid Id) : IRequest<bool>;

public class MarkActivityAsDoneCommandHandler(IRepository<Activity> repo)
    : IRequestHandler<MarkActivityAsDoneCommand, bool>
{
    public async Task<bool> Handle(MarkActivityAsDoneCommand request, CancellationToken ct)
    {
        var activity = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Activity {request.Id} not found.");

        activity.IsDone = true;
        repo.Update(activity);
        await repo.SaveChangesAsync();
        return true;
    }
}



