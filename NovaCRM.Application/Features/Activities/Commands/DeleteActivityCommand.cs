using MediatR;
using NovaCRM.Domain.Entities;
using NovaCRM.Domain.Interfaces;

namespace NovaCRM.Application.Features.Activities.Commands;
public record DeleteActivityCommand(Guid Id) : IRequest<bool>;

public class DeleteActivityCommandHandler(IRepository<Activity> repo)
    : IRequestHandler<DeleteActivityCommand, bool>
{
    public async Task<bool> Handle(DeleteActivityCommand request, CancellationToken ct)
    {
        var activity = await repo.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Activity {request.Id} not found.");

        repo.Delete(activity);
        await repo.SaveChangesAsync();
        return true;
    }
}
